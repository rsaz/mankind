using UnityEditor;
using UnityEngine;

namespace EnhancedHierarchy {
    public class LayerMiniLabel : MiniLabelProvider {

        public override void FillContent(GUIContent content) {
            content.text = EnhancedHierarchy.HasLayer ?
                LayerMask.LayerToName(EnhancedHierarchy.CurrentGameObject.layer) :
                string.Empty;
        }

        public override bool Faded() {
            return EnhancedHierarchy.CurrentGameObject.layer == EnhancedHierarchy.UNLAYERED;
        }

        public override bool Draw(Rect rect, GUIContent content, GUIStyle style) {
            GUI.changed = false;

            var layer = EditorGUI.LayerField(rect, EnhancedHierarchy.CurrentGameObject.layer, Styles.miniLabelStyle);

            if (GUI.changed)
                Icons.Layer.ChangeLayerAndAskForChildren(EnhancedHierarchy.GetSelectedObjectsAndCurrent(), layer);

            return GUI.changed;
        }

        public override void OnClick() { }

    }
}
