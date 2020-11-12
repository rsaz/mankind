using UnityEditor;
using UnityEngine;

namespace EnhancedHierarchy {
    public class TagMiniLabel : MiniLabelProvider {

        public override void FillContent(GUIContent content) {
            content.text = EnhancedHierarchy.HasTag ?
                EnhancedHierarchy.GameObjectTag :
                string.Empty;
        }

        public override bool Faded() {
            return EnhancedHierarchy.GameObjectTag == EnhancedHierarchy.UNTAGGED;
        }

        public override bool Draw(Rect rect, GUIContent content, GUIStyle style) {
            GUI.changed = false;

            var tag = EditorGUI.TagField(rect, EnhancedHierarchy.GameObjectTag, style);

            if (GUI.changed)
                Icons.Tag.ChangeTagAndAskForChildren(EnhancedHierarchy.GetSelectedObjectsAndCurrent(), tag);

            return GUI.changed;
        }

        public override void OnClick() { }

    }
}
