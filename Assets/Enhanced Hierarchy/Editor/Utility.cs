using System;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace EnhancedHierarchy {
    /// <summary>
    /// Misc utilities for Enhanced Hierarchy.
    /// </summary>
    public static class Utility {

        private const string CTRL = "Ctrl";
        private const string CMD = "Cmd";
        private const string MENU_ITEM_PATH = "Edit/Enhanced Hierarchy %h";

        private static int errorCount;
        private static readonly GUIContent tempContent = new GUIContent();

        public static string CtrlKey { get { return Application.platform == RuntimePlatform.OSXEditor ? CMD : CTRL; } }

        [MenuItem(MENU_ITEM_PATH, false, int.MinValue)]
        private static void EnableDisableHierarchy() {
            Preferences.Enabled.Value = !Preferences.Enabled;
            EditorApplication.RepaintHierarchyWindow();
        }

        [MenuItem(MENU_ITEM_PATH, true)]
        private static bool CheckHierarchyEnabled() {
            Menu.SetChecked(MENU_ITEM_PATH, Preferences.Enabled);
            return true;
        }

        public static void EnableFPSCounter() {
            var frames = 0;
            var fps = 0d;
            var lastTime = 0d;
            var content = new GUIContent();
            var evt = EventType.Repaint;

            EditorApplication.hierarchyWindowItemOnGUI += (id, rect) => {
                using(ProfilerSample.Get("Enhanced Hierarchy"))
                using(ProfilerSample.Get("FPS Counter")) {
                    if (evt == Event.current.type)
                        return;

                    evt = Event.current.type;

                    if (evt == EventType.Repaint)
                        frames++;

                    if (EditorApplication.timeSinceStartup - lastTime < 0.5d)
                        return;

                    fps = frames / (EditorApplication.timeSinceStartup - lastTime);
                    lastTime = EditorApplication.timeSinceStartup;
                    frames = 0;

                    content.text = string.Format("{0:00.0} FPS", fps);
                    content.image = Styles.warningIcon;

                    SetHierarchyTitle(content);
                }
            };
        }

        public static bool ShouldCalculateTooltipAt(Rect area) {
            return area.Contains(Event.current.mousePosition);
        }

        public static void ForceUpdateHierarchyEveryFrame() {
            // EditorApplication.update += () => {
            //     if(EditorWindow.mouseOverWindow)
            //         EditorApplication.RepaintHierarchyWindow();
            // };
        }

        public static void LogException(Exception e) {
            Debug.LogError("Unexpected exception in Enhanced Hierarchy");
            Debug.LogException(e);

            if (errorCount++ >= 10) {
                Debug.LogWarning("Automatically disabling Enhanced Hierarchy, if the error persists contact the developer");
                Preferences.Enabled.Value = false;
                errorCount = 0;

                if (!EditorPrefs.GetBool("EHEmailAskDisabled", false))
                    switch (EditorUtility.DisplayDialogComplex("Mail Developer", "Enhanced Hierarchy has found an exeption, would you like to report a bug to the developer? (If you choose yes your mail app will open with a few techinical information)", "Yes", "Not now", "No and don't ask again")) {
                        case 0:
                            Preferences.OpenSupportEmail(e);
                            EditorUtility.DisplayDialog("Mail Developer", "Your mail app will open now, if it doesn't please send an email reporting the bug to " + Preferences.DEVELOPER_EMAIL, "OK");
                            break;
                        case 1:

                            break;
                        case 2:
                            EditorPrefs.SetBool("EHEmailAskDisabled", true);
                            EditorUtility.DisplayDialog("Mail Developer", "You won't be bothered again, sorry", "OK");
                            break;
                    }
            }
        }

        public static void SetHierarchyTitle(string title) {
            try {
                Reflected.HierarchyWindowInstance.titleContent.text = title;
            } catch (Exception e) {
                Debug.LogWarning("Failed to set hierarchy title: " + e);
            }
        }

        public static void SetHierarchyTitle(GUIContent content) {
            try {
                Reflected.HierarchyWindowInstance.titleContent = content;
            } catch (Exception e) {
                Debug.LogWarning("Failed to set hierarchy title: " + e);
            }
        }

        public static GUIStyle CreateStyleFromTextures(Texture2D on, Texture2D off) {
            return CreateStyleFromTextures(null, on, off);
        }

        public static GUIStyle CreateStyleFromTextures(GUIStyle reference, Texture2D on, Texture2D off) {
            using(ProfilerSample.Get()) {
                var style = reference != null ? new GUIStyle(reference) : new GUIStyle();

                style.active.background = off;
                style.focused.background = off;
                style.hover.background = off;
                style.normal.background = off;
                style.onActive.background = on;
                style.onFocused.background = on;
                style.onHover.background = on;
                style.onNormal.background = on;
                style.imagePosition = ImagePosition.ImageOnly;

                EditorApplication.update += () => {
                    style.fixedHeight = Preferences.IconsSize;
                    style.fixedWidth = Preferences.IconsSize;
                };

                return style;
            }
        }

        public static Texture2D GetBackground(GUIStyle style, bool on) {
            return on ?
                style.onNormal.background :
                style.normal.background;
        }

        public static Texture2D FindOrLoad(string base64) {
            var name = string.Format("Enhanced_Hierarchy_{0}", (long)base64.GetHashCode() - int.MinValue);

            return FindTextureFromName(name) ?? LoadTexture(base64, name);
        }

        public static Texture2D LoadTexture(string base64, string name) {
            using(ProfilerSample.Get())
            try {
                var bytes = Convert.FromBase64String(base64);
                var texture = new Texture2D(0, 0, TextureFormat.ARGB32, false, false);

                texture.name = name;
                texture.hideFlags = HideFlags.HideAndDontSave;
                texture.LoadImage(bytes);

                return texture;
            } catch (Exception e) {
                Debug.LogErrorFormat("Failed to load texture \"{0}\": {1}", name, e);
                return null;
            }
        }

        public static Texture2D FindTextureFromName(string name) {
            using(ProfilerSample.Get())
            try {
                var textures = Resources.FindObjectsOfTypeAll<Texture2D>();

                for (var i = 0; i < textures.Length; i++)
                    if (textures[i].name == name)
                        return textures[i];

                return null;
            } catch (Exception e) {
                Debug.LogErrorFormat("Failed to find texture \"{0}\": {1}", name, e);
                return null;
            }
        }

        public static Color GetHierarchyColor(Transform t) {
            if (!t)
                return Color.clear;

            return GetHierarchyColor(t.gameObject);
        }

        public static Color GetHierarchyColor(GameObject go) {
            if (!go)
                return Color.black;

            return GetHierarchyLabelStyle(go).normal.textColor;
        }

        public static GUIStyle GetHierarchyLabelStyle(GameObject go) {
            using(ProfilerSample.Get()) {
                if (!go)
                    return EditorStyles.label;

                var active = go.activeInHierarchy;

                #if UNITY_2018_3_OR_NEWER
                var prefabType = PrefabUtility.GetPrefabInstanceStatus(go);

                switch (prefabType) {
                    case PrefabInstanceStatus.Connected:
                        return active ? Styles.labelPrefab : Styles.labelPrefabDisabled;

                    case PrefabInstanceStatus.MissingAsset:
                        return active ? Styles.labelPrefabBroken : Styles.labelPrefabBrokenDisabled;

                    default:
                        return active ? Styles.labelNormal : Styles.labelDisabled;
                }
                #else
                var prefabType = PrefabUtility.GetPrefabType(PrefabUtility.FindPrefabRoot(go));

                switch (prefabType) {
                    case PrefabType.PrefabInstance:
                    case PrefabType.ModelPrefabInstance:
                        return active ? Styles.labelPrefab : Styles.labelPrefabDisabled;

                    case PrefabType.MissingPrefabInstance:
                        return active ? Styles.labelPrefabBroken : Styles.labelPrefabBrokenDisabled;

                    default:
                        return active ? Styles.labelNormal : Styles.labelDisabled;
                }
                #endif
            }
        }

        public static Color OverlayColors(Color src, Color dst) {
            using(ProfilerSample.Get()) {
                var alpha = dst.a + src.a * (1f - dst.a);
                var result = (dst * dst.a + src * src.a * (1f - dst.a)) / alpha;

                result.a = alpha;

                return result;
            }
        }

        public static bool TransformIsLastChild(Transform t) {
            using(ProfilerSample.Get()) {
                if (!t)
                    return true;

                return t.GetSiblingIndex() == t.parent.childCount - 1;
            }
        }

        public static void ApplyHideFlagsToPrefab(UnityEngine.Object obj) {
            var handle = PrefabUtility.GetPrefabInstanceHandle(obj);

            if (handle)
                handle.hideFlags = obj.hideFlags;
        }

        public static void LockObject(GameObject go) {
            using(ProfilerSample.Get()) {
                go.hideFlags |= HideFlags.NotEditable;
                ApplyHideFlagsToPrefab(go);

                #if UNITY_2019_3_OR_NEWER
                if (!Preferences.AllowPickingLockedObjects)
                    SceneVisibilityManager.instance.DisablePicking(go, false);
                #endif

                EditorUtility.SetDirty(go);
            }
        }

        public static void UnlockObject(GameObject go) {
            using(ProfilerSample.Get()) {
                go.hideFlags &= ~HideFlags.NotEditable;
                ApplyHideFlagsToPrefab(go);

                #if UNITY_2019_3_OR_NEWER
                if (!Preferences.AllowPickingLockedObjects)
                    SceneVisibilityManager.instance.EnablePicking(go, false);
                #endif

                EditorUtility.SetDirty(go);
            }
        }

        // public static void UnlockAllObjects() {
        //     using(ProfilerSample.Get())
        //     foreach (var objRaw in Resources.FindObjectsOfTypeAll<GameObject>()) {
        //         var obj = ObjectOrPrefabInstanceHandle(objRaw);
        //         if (obj && (obj.hideFlags & HideFlags.HideInHierarchy) == 0 && !EditorUtility.IsPersistent(obj))
        //             UnlockObject(obj);
        //     }
        // }

        public static void ApplyPrefabModifications(GameObject go, bool allowCreatingNew) {
            #if UNITY_2018_3_OR_NEWER
            var isPrefab = PrefabUtility.IsPartOfAnyPrefab(go);
            #else
            var isPrefab = PrefabUtility.GetPrefabType(go) == PrefabType.PrefabInstance;
            #endif

            if (isPrefab) {

                #if UNITY_2018_3_OR_NEWER
                var prefab = PrefabUtility.GetNearestPrefabInstanceRoot(go);
                #elif UNITY_2018_2_OR_NEWER
                var prefab = PrefabUtility.GetCorrespondingObjectFromSource(go);
                #else
                var prefab = PrefabUtility.GetPrefabParent(go);
                #endif

                if (!prefab) {
                    Debug.LogError("Prefab asset not valid!");
                    return;
                }

                #if UNITY_2018_3_OR_NEWER
                if (PrefabUtility.GetPrefabInstanceStatus(prefab) == PrefabInstanceStatus.Connected)
                    PrefabUtility.ApplyPrefabInstance(prefab, InteractionMode.UserAction);
                else if (EditorUtility.DisplayDialog("Apply disconnected prefab", "This is a disconnected game object, do you want to try to reconnect to the last prefab asset?", "Try to Reconnect", "Cancel"))
                    PrefabUtility.RevertPrefabInstance(prefab, InteractionMode.UserAction);
                EditorUtility.SetDirty(prefab);
                #else
                var selection = Selection.instanceIDs;
                Selection.activeGameObject = go;

                if (EditorApplication.ExecuteMenuItem("GameObject/Apply Changes To Prefab")) {
                    EditorUtility.SetDirty(prefab);
                    Selection.instanceIDs = selection;
                } else
                    Debug.LogError("Failed to apply prefab modifications");
                #endif
            } else if (allowCreatingNew) {
                var path = EditorUtility.SaveFilePanelInProject("Save prefab", "New Prefab", "prefab", "Save the selected prefab");

                if (!string.IsNullOrEmpty(path))
                    #if UNITY_2018_3_OR_NEWER
                    PrefabUtility.SaveAsPrefabAssetAndConnect(go, path, InteractionMode.UserAction);
                #else
                PrefabUtility.CreatePrefab(path, go, ReplacePrefabOptions.ConnectToPrefab);
                #endif
            }
        }

        public static string EnumFlagsToString(Enum value) {
            using(ProfilerSample.Get())
            try {
                if ((int)(object)value == -1)
                    return "Everything";

                var str = new StringBuilder();
                var separator = ", ";

                foreach (var enumValue in Enum.GetValues(value.GetType())) {
                    var i = (int)enumValue;
                    if (i != 0 && (i & (i - 1)) == 0 && Enum.IsDefined(value.GetType(), i) && (Convert.ToInt32(value) & i) != 0) {
                        str.Append(ObjectNames.NicifyVariableName(enumValue.ToString()));
                        str.Append(separator);
                    }
                }

                if (str.Length > 0)
                    str.Length -= separator.Length;

                return str.ToString();
            } catch (Exception e) {
                if (Preferences.DebugEnabled)
                    Debug.LogException(e);
                return string.Empty;
            }
        }

        public static GUIContent GetTempGUIContent(string text, string tooltip = null, Texture2D image = null) {
            tempContent.text = text;
            tempContent.tooltip = tooltip;
            tempContent.image = image;
            return tempContent;
        }

        public static string SafeGetName(this IconBase icon) {
            try {
                return icon.Name;
            } catch (Exception e) {
                Debug.LogException(e);
                Preferences.ForceDisableButton(icon);
                return string.Empty;
            }
        }

        public static float SafeGetWidth(this IconBase icon) {
            try {
                return icon.Width + (Preferences.IconsSize - 15) / 2;
            } catch (Exception e) {
                Debug.LogException(e);
                Preferences.ForceDisableButton(icon);
                return 0f;
            }
        }

        public static void SafeInit(this IconBase icon) {
            try {
                icon.Init();
            } catch (Exception e) {
                Debug.LogException(e);
                Preferences.ForceDisableButton(icon);
            }
        }

        public static void SafeDoGUI(this IconBase icon, Rect rect) {
            try {
                rect.yMin -= (Preferences.IconsSize - 15) / 2;
                rect.xMin -= (Preferences.IconsSize - 15) / 2;
                icon.DoGUI(rect);
            } catch (Exception e) {
                Debug.LogException(e);
                Preferences.ForceDisableButton(icon);
            }
        }

        public static Rect FlipRectHorizontally(Rect rect) {
            return Rect.MinMaxRect(
                rect.xMax,
                rect.yMin,
                rect.xMin,
                rect.yMax
            );
        }

    }
}
