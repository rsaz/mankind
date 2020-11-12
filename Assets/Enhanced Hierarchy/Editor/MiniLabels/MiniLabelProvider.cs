using System;
using UnityEngine;

namespace EnhancedHierarchy {
    public abstract class MiniLabelProvider {

        private readonly GUIContent content = new GUIContent();

        public static readonly Type[] MiniLabelsTypes = new [] {
            null,
            typeof(TagMiniLabel),
            typeof(LayerMiniLabel),
            typeof(SortingLayerMiniLabel)
        };

        public abstract void FillContent(GUIContent content);
        public abstract bool Faded();
        public abstract void OnClick();

        public void Init() {
            FillContent(content);
        }

        public bool HasValue() {
            return content.text.Length > 0;
        }

        public virtual bool Draw(Rect rect, GUIContent content, GUIStyle style) {
            return GUI.Button(rect, content, style);
        }

        public float Measure() {
            var calculated = Styles.miniLabelStyle.CalcSize(content);
            return calculated.x;
        }

        public void Draw(ref Rect rect) {
            if (!HasValue())
                return;

            var color = EnhancedHierarchy.CurrentColor;
            var alpha = Faded() ? Styles.backgroundColorDisabled.a : Styles.backgroundColorEnabled.a;
            var finalColor = color * new Color(1f, 1f, 1f, alpha);

            using(ProfilerSample.Get())
            using(new GUIContentColor(finalColor)) {
                Styles.miniLabelStyle.fontSize = Preferences.SmallerMiniLabel ? 8 : 9;
                rect.xMin -= Measure();

                if (Draw(rect, content, Styles.miniLabelStyle))
                    OnClick();
            }
        }
    }
}
