using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EnhancedHierarchy {
    public static partial class Preferences {

        public const string DEVELOPER_EMAIL = "samuelschultze@gmail.com";

        public static Action onResetPreferences = new Action(() => { });
        public static readonly List<GUIContent> contents = new List<GUIContent>();

        public static readonly Version pluginVersion = new Version(2, 4, 5);
        public static readonly DateTime pluginDate = new DateTime(2020, 05, 04);

        private static readonly GUIContent resetSettingsContent = new GUIContent("Use Defaults", "Reset all settings to default");
        private static readonly GUIContent unlockAllContent = new GUIContent("Unlock All Objects", "Unlock all objects in the current scene, it's highly recommended to do this when disabling or removing the extension to prevent data loss\nThis might take a few seconds on large scenes");
        private static readonly GUIContent mailDeveloperContent = new GUIContent("Support Email", "Request support from the developer\n\n" + DEVELOPER_EMAIL);
        private static readonly GUIContent versionContent = new GUIContent(string.Format("Version: {0} ({1:d})", pluginVersion, pluginDate));

        private static Vector2 scroll;

        private static ReorderableList leftIconsList, rightIconsList, rowColorsList;

        private static readonly string[] minilabelsNames;

        private static GenericMenu LeftIconsMenu { get { return GetGenericMenuForIcons(LeftIcons, IconBase.AllLeftIcons); } }
        private static GenericMenu RightIconsMenu { get { return GetGenericMenuForIcons(RightIcons, IconBase.AllRightIcons); } }

        private static GenericMenu RowColorsMenu {
            get {
                var menu = new GenericMenu();
                var randomColor = Random.ColorHSV(0f, 1f, 0.5f, 1f, 1f, 1f);

                randomColor.a = 0.3019608f;

                for (var i = 0; i < 32; i++) {
                    if (PerLayerRowColors.Value.Contains(new LayerColor(i)))
                        continue;

                    var mode = PerLayerRowColors.Value.LastOrDefault().mode;
                    var layerName = LayerMask.LayerToName(i);
                    var layer = new LayerColor(i, randomColor, mode);

                    if (string.IsNullOrEmpty(layerName))
                        layerName = string.Format("Layer: {0}", i);

                    menu.AddItem(new GUIContent(layerName), false, () => {
                        rowColorsList.list.Add(layer);
                        PerLayerRowColors.ForceSave();
                    });
                }

                return menu;
            }
        }

        private static GenericMenu GetGenericMenuForIcons<T>(PrefItem<T> preferenceItem, IconBase[] icons)where T : IList {
            var menu = new GenericMenu();

            foreach (var i in icons) {
                var icon = i;

                if (!preferenceItem.Value.Contains(icon) && icon != IconBase.none && icon != IconBase.none)
                    menu.AddItem(new GUIContent(icon.Name), false, () => {
                        preferenceItem.Value.Add(icon);
                        preferenceItem.ForceSave();
                    });

            }

            return menu;
        }

        private static ReorderableList GenerateReordableList<T>(PrefItem<T> preferenceItem)where T : IList {
            var result = new ReorderableList(preferenceItem.Value, typeof(T), true, true, true, true);

            result.elementHeight = 18f;
            result.drawHeaderCallback = rect => { rect.xMin -= EditorGUI.indentLevel * 16f; EditorGUI.LabelField(rect, preferenceItem, EditorStyles.boldLabel); };
            result.onChangedCallback += list => preferenceItem.ForceSave();
            result.drawElementCallback = (rect, index, focused, active) => {
                var icon = result.list[index] as IconBase;

                if (icon == null) {
                    EditorGUI.LabelField(rect, "INVALID ICON");
                    return;
                }

                var content = Utility.GetTempGUIContent(icon.Name, icon.PreferencesTooltip, icon.PreferencesPreview);
                var whiteTexture = content.image ? content.image.name.Contains("eh_icon_white") : true;

                using(new GUIColor((whiteTexture && !EditorGUIUtility.isProSkin) ? Styles.backgroundColorEnabled : Color.white))
                EditorGUI.LabelField(rect, content);
            };

            onResetPreferences += () => result.list = preferenceItem.Value;

            return result;
        }

        #if UNITY_2018_3_OR_NEWER
        [SettingsProvider]
        private static SettingsProvider RetrieveSettingsProvider() {
            var settingsProvider = new SettingsProvider("Preferences/Enhanced Hierarchy", SettingsScope.User, contents.Select(c => c.text));

            settingsProvider.guiHandler = new Action<string>((str) => OnPreferencesGUI(str));

            return settingsProvider;
        }

        #else
        [PreferenceItem("Hierarchy")]
        private static void OnPreferencesGUI() {
            OnPreferencesGUI(string.Empty);
        }
        #endif

        private static void OnPreferencesGUI(string search) {

            scroll = EditorGUILayout.BeginScrollView(scroll, false, false);

            EditorGUILayout.Separator();
            Enabled.DoGUI();
            EditorGUILayout.Separator();
            EditorGUILayout.HelpBox("Each item has a tooltip explaining what it does, keep the mouse over it to see.", MessageType.Info);
            EditorGUILayout.Separator();

            using(Enabled.GetEnabledScope()) {
                using(new GUIIndent("Misc settings")) {

                    using(new GUIIndent("Margins")) {
                        RightMargin.DoGUISlider(-50, 50);
                        using(new GUIEnabled(Reflected.HierarchyArea.Supported)) {
                            LeftMargin.DoGUISlider(-50, 50);
                            Indent.DoGUISlider(0, 35);
                        }
                        if (!Reflected.HierarchyArea.Supported)
                            EditorGUILayout.HelpBox("Custom Indent and Margins are not supported in this Unity version", MessageType.Warning);
                    }

                    IconsSize.DoGUISlider(13, 23);
                    TreeOpacity.DoGUISlider(0f, 1f);

                    using(new GUIIndent()) {
                        using(SelectOnTree.GetFadeScope(TreeOpacity.Value > 0.01f))
                        SelectOnTree.DoGUI();
                        using(TreeStemProportion.GetFadeScope(TreeOpacity.Value > 0.01f))
                        TreeStemProportion.DoGUISlider(0f, 1f);
                    }

                    Tooltips.DoGUI();

                    using(new GUIIndent())
                    using(RelevantTooltipsOnly.GetFadeScope(Tooltips))
                    RelevantTooltipsOnly.DoGUI();

                    if (EnhancedSelectionSupported)
                        EnhancedSelection.DoGUI();

                    Trailing.DoGUI();
                    ChangeAllSelected.DoGUI();
                    NumericChildExpand.DoGUI();

                    using(new GUIEnabled(Reflected.IconWidthSupported))
                    DisableNativeIcon.DoGUI();

                    using(HideDefaultIcon.GetFadeScope(IsButtonEnabled(new Icons.GameObjectIcon())))
                    HideDefaultIcon.DoGUI();

                    using(OpenScriptsOfLogs.GetFadeScope(IsButtonEnabled(new Icons.Warnings())))
                    OpenScriptsOfLogs.DoGUI();

                    GUI.changed = false;

                    using(AllowSelectingLockedObjects.GetFadeScope(IsButtonEnabled(new Icons.Lock())))
                    AllowSelectingLockedObjects.DoGUI();

                    #if !UNITY_2019_3_OR_NEWER
                    using(new GUIEnabled(false))
                    #endif
                    using(AllowPickingLockedObjects.GetFadeScope(IsButtonEnabled(new Icons.Lock())))
                    AllowPickingLockedObjects.DoGUI();

                    HoverTintColor.DoGUI();
                }

                using(new GUIIndent("Row separators")) {
                    LineSize.DoGUISlider(0, 6);

                    using(LineColor.GetFadeScope(LineSize > 0))
                    LineColor.DoGUI();

                    OddRowColor.DoGUI();
                    EvenRowColor.DoGUI();

                    GUI.changed = false;
                    var rect = EditorGUILayout.GetControlRect(false, rowColorsList.GetHeight());
                    rect.xMin += EditorGUI.indentLevel * 16f;
                    rowColorsList.DoList(rect);
                }

                GUI.changed = false;
                MiniLabels.Value[0] = EditorGUILayout.Popup("Mini Label Top", MiniLabels.Value[0], minilabelsNames);
                MiniLabels.Value[1] = EditorGUILayout.Popup("Mini Label Bottom", MiniLabels.Value[1], minilabelsNames);

                if (GUI.changed) {
                    MiniLabels.ForceSave();
                    RecreateMiniLabelProviders();
                }

                // MiniLabel.DoGUI();
                using(new GUIIndent()) {
                    using(SmallerMiniLabel.GetFadeScope(miniLabelProviders.Length > 0))
                    SmallerMiniLabel.DoGUI();
                    using(HideDefaultTag.GetFadeScope(MiniLabelTagEnabled))
                    HideDefaultTag.DoGUI();
                    using(HideDefaultLayer.GetFadeScope(MiniLabelLayerEnabled))
                    HideDefaultLayer.DoGUI();
                    using(CentralizeMiniLabelWhenPossible.GetFadeScope(miniLabelProviders.Length >= 2))
                    CentralizeMiniLabelWhenPossible.DoGUI();
                }

                LeftSideButtonPref.DoGUI();
                using(new GUIIndent())
                using(LeftmostButton.GetFadeScope(LeftSideButton != IconBase.none))
                LeftmostButton.DoGUI();

                using(new GUIIndent("Children behaviour on change")) {
                    using(LockAskMode.GetFadeScope(IsButtonEnabled(new Icons.Lock())))
                    LockAskMode.DoGUI();
                    using(LayerAskMode.GetFadeScope(IsButtonEnabled(new Icons.Layer()) || MiniLabelLayerEnabled))
                    LayerAskMode.DoGUI();
                    using(TagAskMode.GetFadeScope(IsButtonEnabled(new Icons.Tag()) || MiniLabelTagEnabled))
                    TagAskMode.DoGUI();
                    using(StaticAskMode.GetFadeScope(IsButtonEnabled(new Icons.Static())))
                    StaticAskMode.DoGUI();
                    using(IconAskMode.GetFadeScope(IsButtonEnabled(new Icons.GameObjectIcon())))
                    IconAskMode.DoGUI();

                    EditorGUILayout.HelpBox(string.Format("Pressing down {0} while clicking on a button will make it temporary have the opposite children change mode", Utility.CtrlKey), MessageType.Info);
                }

                leftIconsList.displayAdd = LeftIconsMenu.GetItemCount() > 0;
                leftIconsList.DoLayoutList();

                rightIconsList.displayAdd = RightIconsMenu.GetItemCount() > 0;
                rightIconsList.DoLayoutList();

                EditorGUILayout.HelpBox("Alt + Click on child expand toggle makes it expand all the grandchildren too", MessageType.Info);

                if (IsButtonEnabled(new Icons.Memory()))
                    EditorGUILayout.HelpBox("\"Memory Used\" may create garbage and consequently framerate stutterings, leave it disabled if maximum performance is important for your project", MessageType.Warning);

                if (IsButtonEnabled(new Icons.Lock()))
                    EditorGUILayout.HelpBox("Remember to always unlock your game objects when removing or disabling this extension, as you won't be able to unlock without it and may lose scene data", MessageType.Warning);

                GUI.enabled = true;
                EditorGUILayout.EndScrollView();

                using(new EditorGUILayout.HorizontalScope()) {
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.LabelField(versionContent, GUILayout.Width(170f));
                }

                using(new EditorGUILayout.HorizontalScope()) {
                    if (GUILayout.Button(resetSettingsContent, GUILayout.Width(120f)))
                        onResetPreferences();

                    // if (GUILayout.Button(unlockAllContent, GUILayout.Width(120f)))
                    //     Utility.UnlockAllObjects();

                    if (GUILayout.Button(mailDeveloperContent, GUILayout.Width(120f)))
                        OpenSupportEmail();

                }

                EditorGUILayout.Separator();
                Styles.ReloadTooltips();
                EditorApplication.RepaintHierarchyWindow();
            }

        }

        public static void OpenSupportEmail(Exception e = null) {
            Application.OpenURL(GetEmailURL(e));
        }

        private static string GetEmailURL(Exception e) {
            var full = new StringBuilder();
            var body = new StringBuilder();

            #if UNITY_2018_1_OR_NEWER
            Func<string, string> EscapeURL = url => { return UnityEngine.Networking.UnityWebRequest.EscapeURL(url).Replace("+", "%20"); };
            #else
            Func<string, string> EscapeURL = url => { return WWW.EscapeURL(url).Replace("+", "%20"); };
            #endif

            body.Append(EscapeURL("\r\nDescribe your problem or make your request here\r\n"));
            body.Append(EscapeURL("\r\nAdditional Information:"));
            body.Append(EscapeURL("\r\nVersion: " + pluginVersion.ToString(3)));
            body.Append(EscapeURL("\r\nUnity " + InternalEditorUtility.GetFullUnityVersion()));
            body.Append(EscapeURL("\r\n" + SystemInfo.operatingSystem));

            if (e != null)
                body.Append(EscapeURL("\r\n" + e));

            full.Append("mailto:");
            full.Append(DEVELOPER_EMAIL);
            full.Append("?subject=");
            full.Append(EscapeURL("Enhanced Hierarchy - Support"));
            full.Append("&body=");
            full.Append(body);

            return full.ToString();
        }

        private static LayerColor LayerColorField(Rect rect, LayerColor layerColor) {
            var value = layerColor;
            var rect1 = rect;
            var rect2 = rect;
            var rect3 = rect;
            var rect4 = rect;

            rect1.xMax = rect1.xMin + 175f;
            rect2.xMin = rect1.xMax;
            rect2.xMax = rect2.xMin + 80f;
            rect3.xMin = rect2.xMax;
            rect3.xMax = rect3.xMin + 100;
            rect4.xMin = rect3.xMax;

            value.layer = EditorGUI.LayerField(rect1, value.layer);
            value.layer = EditorGUI.DelayedIntField(rect2, value.layer);
            value.color = EditorGUI.ColorField(rect3, value.color);
            value.mode = (TintMode)EditorGUI.EnumPopup(rect4, value.mode);

            if (value.layer > 31 || value.layer < 0)
                return layerColor;

            return value;
        }

        private static void DoGUI(this PrefItem<int> prefItem) {
            if (prefItem.Drawing)
                prefItem.Value = EditorGUILayout.IntField(prefItem, prefItem);
        }

        private static void DoGUI(this PrefItem<float> prefItem) {
            if (prefItem.Drawing)
                prefItem.Value = EditorGUILayout.FloatField(prefItem, prefItem);
        }

        private static void DoGUISlider(this PrefItem<int> prefItem, int min, int max) {
            if (prefItem.Drawing)
                prefItem.Value = EditorGUILayout.IntSlider(prefItem, prefItem, min, max);
        }

        private static void DoGUISlider(this PrefItem<float> prefItem, float min, float max) {
            if (prefItem.Drawing)
                prefItem.Value = EditorGUILayout.Slider(prefItem, prefItem, min, max);
        }

        private static void DoGUI(this PrefItem<bool> prefItem) {
            if (prefItem.Drawing)
                prefItem.Value = EditorGUILayout.Toggle(prefItem, prefItem);
        }

        private static void DoGUI(this PrefItem<string> prefItem) {
            if (prefItem.Drawing)
                prefItem.Value = EditorGUILayout.TextField(prefItem.Label, prefItem);
        }

        private static void DoGUI(this PrefItem<Color> prefItem) {
            if (prefItem.Drawing)
                prefItem.Value = EditorGUILayout.ColorField(prefItem, prefItem);
        }

        private static void DoGUI<T>(this PrefItem<T> prefItem)where T : struct, IConvertible {
            if (prefItem.Drawing)
                prefItem.Value = (T)(object)EditorGUILayout.EnumPopup(prefItem, (Enum)(object)prefItem.Value);
        }

        private static void DoGUI(this PrefItem<IconData> prefItem) {
            if (!prefItem.Drawing)
                return;

            var icons = IconBase.AllLeftOfNameIcons;
            var index = Array.IndexOf(icons, prefItem.Value.Icon);
            var labels = (from icon in icons select new GUIContent(icon)).ToArray();

            index = EditorGUILayout.Popup(prefItem, index, labels);

            if (index < 0 || index >= icons.Length)
                return;

            if (prefItem.Value.Icon.Name == icons[index].Name)
                return;

            prefItem.Value.Icon = icons[index];
            prefItem.ForceSave();
        }
    }
}
