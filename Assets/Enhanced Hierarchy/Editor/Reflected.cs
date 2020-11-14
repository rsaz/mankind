using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EnhancedHierarchy {
    public static partial class Reflected {

        private static bool gameObjectStylesTypeLoaded = false;
        private static Type gameObjectTreeViewStylesType;

        private static readonly Type hierarchyWindowType = ReflectionHelper.FindType("UnityEditor.SceneHierarchyWindow");

        private static EditorWindow hierarchyWindowInstance;

        public static bool HierarchyFocused {
            get {
                return EditorWindow.focusedWindow && EditorWindow.focusedWindow.GetType() == hierarchyWindowType;
            }
        }

        public static Color PlaymodeTint {
            get {
                try {
                    return !EditorApplication.isPlayingOrWillChangePlaymode ?
                        Color.white :
                        ReflectionHelper.FindType("UnityEditor.HostView").GetStaticField<object>("kPlayModeDarken").GetInstanceProperty<Color>("Color");
                } catch (Exception e) {
                    if (Preferences.DebugEnabled)
                        Debug.LogException(e);
                    return Color.white;
                }
            }
        }

        public static EditorWindow HierarchyWindowInstance {
            get {
                if (hierarchyWindowInstance)
                    return hierarchyWindowInstance;

                var lastHierarchy = (EditorWindow)null;

                try {
                    lastHierarchy = hierarchyWindowType.GetStaticField<EditorWindow>("s_LastInteractedHierarchy");
                } catch (Exception e) {
                    if (Preferences.DebugEnabled)
                        Debug.LogException(e);
                }

                return lastHierarchy != null ?
                    (hierarchyWindowInstance = lastHierarchy) :
                    (hierarchyWindowInstance = (EditorWindow)Resources.FindObjectsOfTypeAll(hierarchyWindowType).FirstOrDefault());
            }
        }

        public static void ShowIconSelector(Object[] targetObjs, Rect activatorRect, bool showLabelIcons) {
            using(ProfilerSample.Get())
            try {
                var iconSelectorType = ReflectionHelper.FindType("UnityEditor.IconSelector");

                if (iconSelectorType.HasMethod<Object[], Rect, bool>("ShowAtPosition")) {
                    if (!iconSelectorType.InvokeStaticMethod<bool, Object[], Rect, bool>("ShowAtPosition", targetObjs, activatorRect, showLabelIcons))
                        Debug.LogWarning("Failed to open icon selector");
                    return;
                } else {
                    var instance = ScriptableObject.CreateInstance(iconSelectorType);

                    if (instance.HasMethod<Object[], Rect, bool>("Init"))
                        instance.InvokeMethod("Init", targetObjs, activatorRect, showLabelIcons);
                    else {
                        var affectedObj = targetObjs.FirstOrDefault();
                        instance.InvokeMethod("Init", affectedObj, activatorRect, showLabelIcons);

                        After.Condition(() => !instance, () => {
                            var icon = GetObjectIcon(affectedObj);

                            foreach (var obj in targetObjs)
                                SetObjectIcon(obj, icon);
                        });
                    }
                }
            } catch (Exception e) {
                Debug.LogWarning("Failed to open icon selector\n" + e);
            }
        }

        public static void SetObjectIcon(Object obj, Texture2D texture) {
            using(ProfilerSample.Get()) {
                typeof(EditorGUIUtility).InvokeStaticMethod("SetIconForObject", obj, texture);
                EditorUtility.SetDirty(obj);
            }
        }

        public static Texture2D GetObjectIcon(Object obj) {
            using(ProfilerSample.Get())
            return typeof(EditorGUIUtility).InvokeStaticMethod<Texture2D, Object>("GetIconForObject", obj);
        }

        public static bool GetTransformIsExpanded(GameObject go) {
            using(ProfilerSample.Get())
            try {
                var data = TreeView.GetInstanceProperty<object>("data");
                var isExpanded = data.InvokeMethod<bool, int>("IsExpanded", go.GetInstanceID());

                return isExpanded;
            } catch (Exception e) {
                Preferences.NumericChildExpand.Value = false;
                Debug.LogException(e);
                Debug.LogWarningFormat("Disabled \"{0}\" because it failed to get hierarchy info", Preferences.NumericChildExpand.Label.text);
                return false;
            }
        }

        public static void SetHierarchySelectionNeedSync() {
            using(ProfilerSample.Get())
            try {
                if (HierarchyWindowInstance)
                    SceneHierarchyOrWindow.SetInstanceProperty("selectionSyncNeeded", true);
            } catch (Exception e) {
                Debug.LogWarningFormat("Enabling \"{0}\" because it caused an exception", Preferences.AllowSelectingLockedObjects.Label.text);
                Debug.LogException(e);
                Preferences.AllowSelectingLockedObjects.Value = true;
            }
        }

        private static object SceneHierarchy { get { return HierarchyWindowInstance.GetInstanceProperty<object>("sceneHierarchy"); } }

        private static object SceneHierarchyOrWindow {
            get {
                #if UNITY_2018_3_OR_NEWER
                return HierarchyWindowInstance.GetInstanceProperty<object>("sceneHierarchy");
                #else
                return HierarchyWindowInstance;
                #endif
            }
        }

        public static object TreeView { get { return SceneHierarchyOrWindow.GetInstanceProperty<object>("treeView"); } }

        public static object TreeViewGUI { get { return TreeView.GetInstanceProperty<object>("gui"); } }

        public static bool IconWidthSupported {
            get {
                #if UNITY_2018_3_OR_NEWER
                return TreeView != null && TreeViewGUI != null && TreeViewGUI.HasField("k_IconWidth");
                #else
                return false;
                #endif
            }
        }

        // Icon to the left side of obj name, introducted on Unity 2018.3
        public static float IconWidth {
            get {
                if (!IconWidthSupported)
                    return 0;
                return TreeViewGUI.GetInstanceField<float>("k_IconWidth");
            }
            set { TreeViewGUI.SetInstanceField("k_IconWidth", value); }
        }

        public static class HierarchyArea {

            static HierarchyArea() {
                if (Preferences.DebugEnabled && !Supported)
                    Debug.LogWarning("HierarchyArea not supported!");
            }

            public static bool Supported {
                get {
                    try {
                        return HierarchyWindowInstance != null && TreeView != null && TreeViewGUI != null;
                    } catch {
                        return false;
                    }
                }
            }

            public static float IndentWidth {
                get { return TreeViewGUI.GetInstanceField<float>("k_IndentWidth"); }
                set { TreeViewGUI.SetInstanceField("k_IndentWidth", value); }
            }

            //public static float foldoutYOffset {
            //    get { return TreeViewGUI.GetFieldValue<float>("foldoutYOffset"); }
            //    set { TreeViewGUI.SetFieldValue("foldoutYOffset", value); }
            //}

            private static float baseIndentDefault = float.NaN;

            public static float BaseIndent {
                get {
                    var val = TreeViewGUI.GetInstanceField<float>("k_BaseIndent");
                    if (float.IsNaN(baseIndentDefault))
                        baseIndentDefault = val;
                    return val;
                }
                set {
                    if (float.IsNaN(baseIndentDefault))
                        baseIndentDefault = BaseIndent;
                    TreeViewGUI.SetInstanceField("k_BaseIndent", baseIndentDefault + value);
                }
            }

            public static float BottomRowMargin {
                get { return TreeViewGUI.GetInstanceField<float>("k_BottomRowMargin"); }
                set { TreeViewGUI.SetInstanceField("k_BottomRowMargin", value); }
            }

            public static float TopRowMargin {
                get { return TreeViewGUI.GetInstanceField<float>("k_TopRowMargin"); }
                set { TreeViewGUI.SetInstanceField("k_TopRowMargin", value); }
            }

            public static float HalfDropBetweenHeight {
                get { return TreeViewGUI.GetInstanceField<float>("k_HalfDropBetweenHeight"); }
                set { TreeViewGUI.SetInstanceField("k_HalfDropBetweenHeight", value); }
            }

            public static float IconWidth {
                get { return TreeViewGUI.GetInstanceField<float>("k_IconWidth"); }
                set { TreeViewGUI.SetInstanceField("k_IconWidth", value); }
            }

            public static float LineHeight {
                get { return TreeViewGUI.GetInstanceField<float>("k_LineHeight"); }
                set { TreeViewGUI.SetInstanceField("k_LineHeight", value); }
            }

            public static float SpaceBetweenIconAndText {
                get { return TreeViewGUI.GetInstanceField<float>("k_SpaceBetweenIconAndText"); }
                set { TreeViewGUI.SetInstanceField("k_SpaceBetweenIconAndText", value); }
            }

            public static float IconLeftPadding {
                get { return TreeViewGUI.GetInstanceProperty<float>("iconLeftPadding"); }
                set { TreeViewGUI.SetInstanceProperty("iconLeftPadding", value); }
            }

            public static float IconRightPadding { //Same as iconLeftPadding
                get { return TreeViewGUI.GetInstanceProperty<float>("iconRightPadding"); }
                set { TreeViewGUI.SetInstanceProperty("iconRightPadding", value); }
            }
        }

        private static Type GameObjectTreeViewStylesType {
            get {
                if (!gameObjectStylesTypeLoaded) {
                    gameObjectStylesTypeLoaded = true;
                    gameObjectTreeViewStylesType = TreeViewGUI.GetType().GetNestedType("GameObjectStyles", ReflectionHelper.FULL_BINDING);
                }

                return gameObjectTreeViewStylesType;
            }
        }

        public static bool NativeHierarchyHoverTintSupported {
            get {
                return GameObjectTreeViewStylesType != null && GameObjectTreeViewStylesType.HasField("hoveredBackgroundColor");
            }
        }

        // I implement the hover tint and then a few weeks later
        // unity implements it as a native feature
        public static Color NativeHierarchyHoverTint {
            get {
                if (Preferences.DebugEnabled && !NativeHierarchyHoverTintSupported) {
                    Debug.LogWarning("Native hover tint not supported!");
                    return Color.clear;
                }

                return GameObjectTreeViewStylesType.GetStaticField<Color>("hoveredBackgroundColor");
            }
            set {
                if (Preferences.DebugEnabled && !NativeHierarchyHoverTintSupported) {
                    Debug.LogWarning("Native hover tint not supported!");
                    return;
                }

                GameObjectTreeViewStylesType.SetStaticField("hoveredBackgroundColor", value);
            }
        }

    }
}
