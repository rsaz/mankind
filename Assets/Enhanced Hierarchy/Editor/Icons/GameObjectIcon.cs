using System.Linq;
using UnityEditor;
using UnityEngine;

namespace EnhancedHierarchy.Icons {
    public sealed class GameObjectIcon : IconBase {

        private GUIContent lastContent;

        public override string Name { get { return "GameObject Icon"; } }
        public override IconPosition Side { get { return IconPosition.All; } }
        public override float Width { get { return lastContent.image ? base.Width : 0f; } }

        public override Texture2D PreferencesPreview { get { return AssetPreview.GetMiniTypeThumbnail(typeof(GameObject)); } }

        //public override string PreferencesTooltip { get { return "Some tag for the tooltip here"; } }

        public override void Init() {
            if (lastContent == null)
                lastContent = new GUIContent();

            lastContent.text = string.Empty;

            lastContent.image = Preferences.HideDefaultIcon ? // Fix #46
                Reflected.GetObjectIcon(EnhancedHierarchy.CurrentGameObject) :
                AssetPreview.GetMiniThumbnail(EnhancedHierarchy.CurrentGameObject);

            lastContent.tooltip = Preferences.Tooltips && !Preferences.RelevantTooltipsOnly ? "Change Icon" : string.Empty;
        }

        public override void DoGUI(Rect rect) {
            using(ProfilerSample.Get()) {
                rect.yMin++;
                rect.xMin++;

                GUI.changed = false;
                GUI.Button(rect, lastContent, EditorStyles.label);

                if (!GUI.changed)
                    return;

                var affectedObjsList = GetSelectedObjectsAndCurrent();
                var affectedObjsEnum = affectedObjsList.AsEnumerable();
                var changeMode = AskChangeModeIfNecessary(affectedObjsList, Preferences.IconAskMode.Value, "Change Icons", "Do you want to change children icons as well?");

                switch (changeMode) {
                    case ChildrenChangeMode.ObjectAndChildren:
                        affectedObjsEnum = affectedObjsEnum.SelectMany(go => go.GetComponentsInChildren<Transform>(true).Select(t => t.gameObject));
                        break;
                }

                affectedObjsEnum = affectedObjsEnum.Distinct();

                var affectedObjsArray = affectedObjsEnum.ToArray();

                foreach (var obj in affectedObjsArray)
                    Undo.RegisterCompleteObjectUndo(obj, "Icon Changed");

                Reflected.ShowIconSelector(affectedObjsArray, rect, true);
            }
        }

    }
}
