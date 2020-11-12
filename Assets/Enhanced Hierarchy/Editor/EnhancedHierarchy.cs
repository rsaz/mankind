using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EnhancedHierarchy {
    /// <summary>
    /// Main class, draws hierarchy items.
    /// </summary>
    [InitializeOnLoad]
    public static partial class EnhancedHierarchy {

        private static MiniLabelProvider[] MiniLabelProviders {
            get { return Preferences.miniLabelProviders; }
        }

        static EnhancedHierarchy() {
            if (Preferences.DebugEnabled || Preferences.ProfilingEnabled) {
                Utility.EnableFPSCounter();
                Utility.ForceUpdateHierarchyEveryFrame();
            }

            EditorApplication.hierarchyWindowItemOnGUI += SetItemInformation;
            EditorApplication.hierarchyWindowItemOnGUI += OnItemGUI;
            EditorApplication.RepaintHierarchyWindow();
        }

        private static void OnItemGUI(int id, Rect rect) {
            if (!Preferences.Enabled)
                return;

            using(ProfilerSample.Get("Enhanced Hierarchy"))
            try {
                if (IsGameObject) {
                    for (var i = 0; i < Preferences.RightIcons.Value.Count; i++)
                        Preferences.RightIcons.Value[i].SafeInit();

                    for (var i = 0; i < Preferences.LeftIcons.Value.Count; i++)
                        Preferences.LeftIcons.Value[i].SafeInit();

                    Preferences.LeftSideButton.SafeInit();

                    for (var i = 0; i < MiniLabelProviders.Length; i++) {
                        MiniLabelProviders[i].Init();
                    }
                }

                if (IsFirstVisible && Reflected.HierarchyArea.Supported) {
                    Reflected.HierarchyArea.IndentWidth = Preferences.Indent;
                    Reflected.HierarchyArea.BaseIndent = Preferences.LeftMargin;
                }

                //SetTitle("EH 2.0");
                CalculateIconsWidth();
                DoSelection(RawRect);
                IgnoreLockedSelection();
                DrawTree(RawRect);
                ChildToggle();
                var trailingWidth = DoTrailing();
                DrawHover();
                ColorSort(RawRect);
                DrawLeftSideIcons(RawRect);
                DrawTooltip(RawRect, trailingWidth);

                if (Reflected.IconWidthSupported)
                    Reflected.IconWidth = Preferences.DisableNativeIcon ? 0 : 16;

                if (IsGameObject) {
                    rect.xMax -= Preferences.RightMargin;
                    rect.xMin = rect.xMax;
                    rect.y++;

                    for (var i = 0; i < Preferences.RightIcons.Value.Count; i++)
                        using(new GUIBackgroundColor(Styles.backgroundColorEnabled)) {
                            var icon = Preferences.RightIcons.Value[i];
                            rect.xMin -= icon.SafeGetWidth();
                            icon.SafeDoGUI(rect);
                            rect.xMax -= icon.SafeGetWidth();
                        }

                    var leftSideRect = RawRect;

                    if (Preferences.LeftmostButton)
                        leftSideRect.xMin = 0f;
                    else
                        leftSideRect.xMin -= 2f + CurrentGameObject.transform.childCount > 0 || Preferences.TreeOpacity > ALPHA_THRESHOLD ? 30f : 18f;

                    leftSideRect.xMax = leftSideRect.xMin + Preferences.LeftSideButton.SafeGetWidth();

                    using(new GUIBackgroundColor(Styles.backgroundColorEnabled))
                    Preferences.LeftSideButton.SafeDoGUI(leftSideRect);
                }

                DrawMiniLabel(ref rect);
                DrawHorizontalSeparator(RawRect);
            } catch (Exception e) {
                Utility.LogException(e);
            }
        }

        private static void DrawHover() {
            if (Reflected.NativeHierarchyHoverTintSupported) {

                if (IsFirstVisible && IsRepaintEvent)
                    Reflected.NativeHierarchyHoverTint = Preferences.HoverTintColor;

                return;
            }

            var tint = Preferences.HoverTintColor.Value;

            if (IsFirstVisible && Reflected.NativeHierarchyHoverTintSupported)
                Reflected.HierarchyWindowInstance.wantsMouseMove = tint.a >= ALPHA_THRESHOLD;

            if (tint.a < ALPHA_THRESHOLD)
                return;

            if (!Utility.ShouldCalculateTooltipAt(FullSizeRect))
                return;

            if (IsRepaintEvent)
                EditorGUI.DrawRect(FullSizeRect, tint);

            switch (Event.current.type) {
                case EventType.MouseMove:
                    Event.current.Use();
                    break;
            }
        }

        private static void IgnoreLockedSelection() {
            if (Preferences.AllowSelectingLockedObjects || !IsFirstVisible || !IsRepaintEvent)
                return;

            using(ProfilerSample.Get()) {
                var selection = Selection.objects;
                var changed = false;

                for (var i = 0; i < selection.Length; i++)
                    if (selection[i] is GameObject && (selection[i].hideFlags & HideFlags.NotEditable) != 0 && !EditorUtility.IsPersistent(selection[i])) {
                        selection[i] = null;
                        changed = true;
                    }

                if (changed) {
                    Selection.objects = selection;
                    Reflected.SetHierarchySelectionNeedSync();
                    EditorApplication.RepaintHierarchyWindow();
                }
            }
        }

        private static void ChildToggle() {
            using(ProfilerSample.Get()) {
                if (!Preferences.NumericChildExpand || !IsRepaintEvent || !IsGameObject || CurrentGameObject.transform.childCount <= 0)
                    return;

                var rect = RawRect;
                var childString = CurrentGameObject.transform.childCount.ToString("00");
                var expanded = Reflected.GetTransformIsExpanded(CurrentGameObject);

                rect.xMax = rect.xMin - 1f;
                rect.xMin -= 15f;

                if (childString.Length > 2)
                    rect.xMin -= 4f;

                using(new GUIBackgroundColor(Styles.childToggleColor))
                Styles.newToggleStyle.Draw(rect, Utility.GetTempGUIContent(childString), false, false, expanded, false);
            }
        }

        private static void DrawHorizontalSeparator(Rect rect) {
            if (Preferences.LineSize < 1 || Preferences.LineColor.Value.a <= ALPHA_THRESHOLD || !IsRepaintEvent)
                return;

            using(ProfilerSample.Get()) {
                rect.xMin = 0f;
                rect.xMax = rect.xMax + 50f;
                rect.yMin -= Preferences.LineSize / 2;
                rect.yMax = rect.yMin + Preferences.LineSize;

                EditorGUI.DrawRect(rect, Preferences.LineColor);

                if (!IsFirstVisible)
                    return;

                rect.y = FinalRect.y - Preferences.LineSize / 2;

                var height = Reflected.HierarchyWindowInstance.position.height;
                var count = (height - FinalRect.y) / FinalRect.height;

                if (FinalRect.height <= 0f)
                    count = 100f;

                for (var i = 0; i < count; i++) {
                    rect.y += RawRect.height;
                    EditorGUI.DrawRect(rect, Preferences.LineColor);
                }
            }
        }

        private static void ColorSort(Rect rect) {
            if (!IsRepaintEvent)
                return;

            using(ProfilerSample.Get()) {
                rect.xMin = 0f;
                rect.xMax = rect.xMax + 50f;

                var rowTint = GetRowTint();
                var rowCustomTint = GetRowCustomTint();

                if (rowCustomTint.color.a > ALPHA_THRESHOLD)
                    using(new GUIColor(rowCustomTint.color)) {
                        switch (rowCustomTint.mode) {
                            case TintMode.Flat:
                                EditorGUI.DrawRect(rect, Color.white);
                                break;
                            case TintMode.GradientLeftToRight:
                                GUI.DrawTexture(Utility.FlipRectHorizontally(rect), Styles.fadeTexture, ScaleMode.StretchToFill);
                                break;
                            case TintMode.GradientRightToLeft:
                                GUI.DrawTexture(rect, Styles.fadeTexture, ScaleMode.StretchToFill);
                                break;
                        }
                    }

                if (rowTint.a > ALPHA_THRESHOLD)
                    EditorGUI.DrawRect(rect, rowTint);

                if (!IsFirstVisible)
                    return;

                rect.y = FinalRect.y;

                var height = Reflected.HierarchyWindowInstance.position.height;
                var count = (height - FinalRect.y) / FinalRect.height;

                if (FinalRect.height <= 0f)
                    count = 100f;

                for (var i = 0; i < count; i++) {
                    rect.y += RawRect.height;
                    rowTint = GetRowTint(rect);

                    if (rowTint.a > ALPHA_THRESHOLD)
                        EditorGUI.DrawRect(rect, rowTint);
                }
            }
        }

        private static void DrawTree(Rect rect) {
            if (Preferences.TreeOpacity <= ALPHA_THRESHOLD || !IsGameObject)
                return;

            if (!IsRepaintEvent && !Preferences.SelectOnTree)
                return;

            using(ProfilerSample.Get())
            using(new GUIColor(Utility.GetHierarchyColor(CurrentGameObject.transform.parent), Preferences.TreeOpacity)) {

                var indent = Reflected.HierarchyArea.Supported ?
                    Reflected.HierarchyArea.IndentWidth :
                    16f;

                // #42 - Jules: pull back indent one level and neatly align tree lines with expansion arrow tips
                rect.x -= indent + 2f;

                rect.xMin -= 14f;
                rect.xMax = rect.xMin + 14f;

                // #42 - Jules: allow showing of stems for container objects
                if (CurrentGameObject.transform.parent) {
                    var lastInHierarchy = Utility.TransformIsLastChild(CurrentGameObject.transform);

                    GUI.DrawTexture(rect, lastInHierarchy ? Styles.treeElbowTexture : Styles.treeTeeTexture);

                    /*
                    	#42 - Jules: add short horizontal line to extend stem if there's no expansion triangle.

                    	NOTE: Please make this value into a slider in the preferences, since it's arguably a bad thing to have it.
                    	It looks "nicer" with extended stems but is less clear since it throws off the stem alignments. 
                    	At a value of 1 this can make things look an entire level off of where they truly are relative to other things!!!
                    	I think 0.5 is an okay compromise, but 0 is the most consistent, which means no extra stem line at all.
                    	So it's the sort of thing where people might have different ideas of what's best for them...
                    */

                    var extendStemProportion = CurrentGameObject.transform.childCount == 0 ?
                        Preferences.TreeStemProportion.Value * indent :
                        indent - 14f;

                    if (extendStemProportion > 0.01f) {
                        var extendedStemRect = new Rect(rect.x + rect.size.x, rect.y + (lastInHierarchy ? 9f : 8f), extendStemProportion, 1f);
                        EditorGUI.DrawRect(extendedStemRect, Color.white);
                    }

                    if (Preferences.SelectOnTree && GUI.Button(rect, GUIContent.none, Styles.labelNormal))
                        Selection.activeTransform = CurrentGameObject.transform.parent;
                }

                var currentTransform = CurrentGameObject.transform.parent;

                for (rect.x -= indent; rect.xMin > 0f && currentTransform && currentTransform.parent; rect.x -= indent) {
                    if (!Utility.TransformIsLastChild(currentTransform))
                        using(new GUIColor(Utility.GetHierarchyColor(currentTransform.parent), Preferences.TreeOpacity)) {
                            GUI.DrawTexture(rect, Styles.treeLineTexture);

                            if (Preferences.SelectOnTree && GUI.Button(rect, GUIContent.none, Styles.labelNormal))
                                Selection.activeTransform = currentTransform.parent;
                        }

                    currentTransform = currentTransform.parent;
                }
            }
        }

        private static void CalculateIconsWidth() {
            using(ProfilerSample.Get()) {
                LeftIconsWidth = 0f;
                RightIconsWidth = 0f;

                if (!IsGameObject)
                    return;

                for (var i = 0; i < Preferences.RightIcons.Value.Count; i++)
                    RightIconsWidth += Preferences.RightIcons.Value[i].SafeGetWidth();

                for (var i = 0; i < Preferences.LeftIcons.Value.Count; i++)
                    LeftIconsWidth += Preferences.LeftIcons.Value[i].SafeGetWidth();
            }
        }

        private static void DrawLeftSideIcons(Rect rect) {
            if (!IsGameObject)
                return;

            using(ProfilerSample.Get()) {
                rect.xMin += LabelSize;
                rect.xMin = Math.Min(rect.xMax - RightIconsWidth - LeftIconsWidth - CalcMiniLabelSize() - 5f - Preferences.RightMargin, rect.xMin);

                for (var i = 0; i < Preferences.LeftIcons.Value.Count; i++)
                    using(new GUIBackgroundColor(Styles.backgroundColorEnabled)) {
                        var icon = Preferences.LeftIcons.Value[i];

                        rect.xMax = rect.xMin + icon.SafeGetWidth();
                        icon.SafeDoGUI(rect);
                        rect.xMin = rect.xMax;
                    }
            }
        }

        private static float DoTrailing() {
            if (!IsRepaintEvent || !Preferences.Trailing || !IsGameObject)
                return RawRect.xMax;

            using(ProfilerSample.Get()) {
                var size = LabelSize; // CurrentStyle.CalcSize(Utility.GetTempGUIContent(GameObjectName)).x;
                var iconsWidth = RightIconsWidth + LeftIconsWidth + CalcMiniLabelSize() + Preferences.RightMargin;

                var iconsMin = FullSizeRect.xMax - iconsWidth;
                var labelMax = LabelOnlyRect.xMax;

                var overlapping = iconsMin <= labelMax;

                if (!overlapping)
                    return labelMax;

                var rect = FullSizeRect;

                rect.xMin = iconsMin - 18;
                rect.xMax = labelMax;

                if (Selection.gameObjects.Contains(CurrentGameObject))
                    EditorGUI.DrawRect(rect, Reflected.HierarchyFocused ? Styles.selectedFocusedColor : Styles.selectedUnfocusedColor);
                else
                    EditorGUI.DrawRect(rect, Styles.normalColor);

                rect.y++;

                using(new GUIColor(CurrentColor))
                EditorStyles.boldLabel.Draw(rect, trailingContent, 0);
                return iconsMin;
            }
        }

        private static void DrawMiniLabel(ref Rect rect) {
            if (!IsGameObject)
                return;

            rect.x -= 3f;

            using(ProfilerSample.Get())
            switch (MiniLabelProviders.Length) {
                case 0:
                    return;

                case 1:
                    if (MiniLabelProviders[0].HasValue())
                        MiniLabelProviders[0].Draw(ref rect);
                    break;

                default:
                    var ml0 = MiniLabelProviders[0];
                    var ml1 = MiniLabelProviders[1];
                    var ml0HasValue = ml0.HasValue();
                    var ml1HasValue = ml1.HasValue();

                    if (ml0HasValue && ml1HasValue || !Preferences.CentralizeMiniLabelWhenPossible) {
                        var topRect = rect;
                        var bottomRect = rect;

                        topRect.yMax = RawRect.yMax - RawRect.height / 2f;
                        bottomRect.yMin = RawRect.yMin + RawRect.height / 2f;

                        if (ml0HasValue)
                            ml0.Draw(ref topRect);
                        if (ml1HasValue)
                            ml1.Draw(ref bottomRect);

                        rect.xMin = Mathf.Min(topRect.xMin, bottomRect.xMin);
                    } else if (ml1HasValue)
                        ml1.Draw(ref rect);
                    else if (ml0HasValue)
                        ml0.Draw(ref rect);
                    break;
            }
        }

        private static float CalcMiniLabelSize() {
            Styles.miniLabelStyle.fontSize = Preferences.SmallerMiniLabel ? 8 : 9;

            using(ProfilerSample.Get()) {
                switch (MiniLabelProviders.Length) {
                    case 0:
                        return 0f;
                    case 1:
                        return MiniLabelProviders[0].Measure();
                    default:
                        return Math.Max(
                            MiniLabelProviders[0].Measure(),
                            MiniLabelProviders[1].Measure()
                        );
                }
            }
        }

        private static void DrawTooltip(Rect rect, float fullTrailingWidth) {
            if (!Preferences.Tooltips || !IsGameObject || !IsRepaintEvent)
                return;

            using(ProfilerSample.Get()) {
                if (DragSelection != null)
                    return;

                rect.xMax = Mathf.Min(fullTrailingWidth, rect.xMin + LabelSize);
                rect.xMin = 0f;

                if (!Utility.ShouldCalculateTooltipAt(rect))
                    return;

                var tooltip = new StringBuilder(100);

                tooltip.AppendLine(GameObjectName);
                tooltip.AppendFormat("\nTag: {0}", GameObjectTag);
                tooltip.AppendFormat("\nLayer: {0}", LayerMask.LayerToName(CurrentGameObject.layer));

                if (GameObjectUtility.GetStaticEditorFlags(CurrentGameObject) != 0)
                    tooltip.AppendFormat("\nStatic: {0}", Utility.EnumFlagsToString(GameObjectUtility.GetStaticEditorFlags(CurrentGameObject)));

                tooltip.AppendLine();
                tooltip.AppendLine();

                foreach (var component in Components)
                    if (component is Transform)
                        continue;
                    else if (component)
                    tooltip.AppendLine(ObjectNames.GetInspectorTitle(component));
                else
                    tooltip.AppendLine("Missing Component");

                EditorGUI.LabelField(rect, Utility.GetTempGUIContent(null, tooltip.ToString().TrimEnd('\n', '\r')));
            }
        }

        private static void DoSelection(Rect rect) {
            if (!Preferences.EnhancedSelectionSupported || !Preferences.EnhancedSelection || Event.current.button != 1) {
                DragSelection = null;
                return;
            }

            using(ProfilerSample.Get()) {
                rect.xMin = 0f;

                switch (Event.current.type) {
                    case EventType.MouseDrag:
                        if (!IsFirstVisible)
                            return;

                        if (DragSelection == null) {
                            DragSelection = new List<Object>();
                            SelectionStart = Event.current.mousePosition;
                            SelectionRect = new Rect();
                        }

                        SelectionRect = new Rect() {
                            xMin = Mathf.Min(Event.current.mousePosition.x, SelectionStart.x),
                            yMin = Mathf.Min(Event.current.mousePosition.y, SelectionStart.y),
                            xMax = Mathf.Max(Event.current.mousePosition.x, SelectionStart.x),
                            yMax = Mathf.Max(Event.current.mousePosition.y, SelectionStart.y)
                        };

                        if (Event.current.control || Event.current.command)
                            DragSelection.AddRange(Selection.objects);

                        Selection.objects = DragSelection.ToArray();
                        Event.current.Use();
                        break;

                    case EventType.MouseUp:
                        if (DragSelection != null)
                            Event.current.Use();
                        DragSelection = null;
                        break;

                    case EventType.Repaint:
                        if (DragSelection == null || !IsFirstVisible)
                            break;

                        Rect scrollRect;

                        if (Event.current.mousePosition.y > FinalRect.y) {
                            scrollRect = FinalRect;
                            scrollRect.y += scrollRect.height;
                        } else if (Event.current.mousePosition.y < RawRect.y) {
                            scrollRect = RawRect;
                            scrollRect.y -= scrollRect.height;
                        } else
                            break;

                        SelectionRect = new Rect() {
                            xMin = Mathf.Min(scrollRect.xMax, SelectionStart.x),
                            yMin = Mathf.Min(scrollRect.yMax, SelectionStart.y),
                            xMax = Mathf.Max(scrollRect.xMax, SelectionStart.x),
                            yMax = Mathf.Max(scrollRect.yMax, SelectionStart.y)
                        };

                        if (Event.current.control || Event.current.command)
                            DragSelection.AddRange(Selection.objects);

                        Selection.objects = DragSelection.ToArray();

                        GUI.ScrollTowards(scrollRect, 9f);
                        EditorApplication.RepaintHierarchyWindow();
                        break;

                    case EventType.Layout:
                        if (DragSelection != null && IsGameObject)
                            if (!SelectionRect.Overlaps(rect) && DragSelection.Contains(CurrentGameObject))
                                DragSelection.Remove(CurrentGameObject);
                            else if (SelectionRect.Overlaps(rect) && !DragSelection.Contains(CurrentGameObject))
                            DragSelection.Add(CurrentGameObject);
                        break;
                }
            }
        }

        public static Color GetRowTint() {
            return GetRowTint(RawRect);
        }

        public static Color GetRowTint(Rect rect) {
            using(ProfilerSample.Get())
            if (rect.y / RawRect.height % 2 >= 0.5f)
                return Preferences.OddRowColor;
            else
                return Preferences.EvenRowColor;
        }

        public static LayerColor GetRowCustomTint() {
            return GetRowCustomTint(CurrentGameObject);
        }

        public static LayerColor GetRowCustomTint(GameObject go) {
            using(ProfilerSample.Get()) {
                if (!go)
                    return new LayerColor();

                var layerColors = Preferences.PerLayerRowColors.Value;

                if (layerColors == null)
                    return new LayerColor();

                var goLayer = go.layer;

                for (var i = 0; i < layerColors.Count; i++)
                    if (layerColors[i] == goLayer)
                        return layerColors[i];

                return new LayerColor();
            }
        }

        public static List<GameObject> GetSelectedObjectsAndCurrent() {
            if (!Preferences.ChangeAllSelected || Selection.gameObjects.Length <= 1)
                return new List<GameObject> { CurrentGameObject };

            var selection = new List<GameObject>(Selection.gameObjects);

            for (var i = 0; i < selection.Count; i++)
                if (EditorUtility.IsPersistent(selection[i]))
                    selection.RemoveAt(i);

            if (!selection.Contains(CurrentGameObject))
                selection.Add(CurrentGameObject);

            selection.Remove(null);
            return selection;
        }
    }
}
