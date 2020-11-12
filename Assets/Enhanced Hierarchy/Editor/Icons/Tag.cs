using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EnhancedHierarchy.Icons {
    public sealed class Tag : IconBase {

        public override IconPosition Side { get { return IconPosition.All; } }

        public override Texture2D PreferencesPreview { get { return Utility.GetBackground(Styles.tagStyle, true); } }

        //public override string PreferencesTooltip { get { return "Some tag for the tooltip here"; } }

        public override void DoGUI(Rect rect) {
            GUI.changed = false;

            EditorGUI.LabelField(rect, Styles.tagContent);
            var tag = EditorGUI.TagField(rect, Styles.tagContent, EnhancedHierarchy.GameObjectTag, Styles.tagStyle);

            if (GUI.changed && tag != EnhancedHierarchy.GameObjectTag)
                ChangeTagAndAskForChildren(GetSelectedObjectsAndCurrent(), tag);
        }

        public static void ChangeTagAndAskForChildren(List<GameObject> objs, string newTag) {
            var changeMode = AskChangeModeIfNecessary(objs, Preferences.TagAskMode, "Change Layer",
                "Do you want to change the tags of the children objects as well?");

            switch (changeMode) {
                case ChildrenChangeMode.ObjectOnly:
                    foreach (var obj in objs) {
                        Undo.RegisterCompleteObjectUndo(obj, "Tag changed");
                        obj.tag = newTag;
                    }
                    break;

                case ChildrenChangeMode.ObjectAndChildren:
                    foreach (var obj in objs) {
                        Undo.RegisterFullObjectHierarchyUndo(obj, "Tag changed");

                        obj.tag = newTag;
                        foreach (var transform in obj.GetComponentsInChildren<Transform>(true))
                            transform.tag = newTag;
                    }
                    break;
            }
        }

    }
}
