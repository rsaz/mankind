using UnityEditor;
using UnityEngine;

namespace EnhancedHierarchy.Icons {
    public sealed class PrefabApply : IconBase {

        public override string Name { get { return "Apply Prefab"; } }
        public override IconPosition Side { get { return IconPosition.All; } }

        public override void DoGUI(Rect rect) {

            #if UNITY_2018_3_OR_NEWER
            var isPrefab = PrefabUtility.IsPartOfAnyPrefab(EnhancedHierarchy.CurrentGameObject);
            #else
            var isPrefab = PrefabUtility.GetPrefabType(EnhancedHierarchy.CurrentGameObject) == PrefabType.PrefabInstance;
            #endif

            using(new GUIContentColor(isPrefab ? Styles.backgroundColorEnabled : Styles.backgroundColorDisabled))
            if (GUI.Button(rect, Styles.prefabApplyContent, Styles.applyPrefabStyle)) {
                var objs = GetSelectedObjectsAndCurrent();

                foreach (var obj in objs)
                    Utility.ApplyPrefabModifications(obj, objs.Count <= 1);
            }
        }

    }
}
