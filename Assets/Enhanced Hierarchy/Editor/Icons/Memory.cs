using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;

namespace EnhancedHierarchy.Icons {
    public sealed class Memory : IconBase {

        private float m_width = 0f;
        private static readonly GUIContent label = new GUIContent();

        public override string Name { get { return "Memory Used"; } }
        public override float Width { get { return m_width + 4f; } }

        private GUIStyle Style { get { return Styles.applyPrefabStyle; } }

        public override void Init() {
            m_width = 0f;

            if (!EnhancedHierarchy.IsGameObject)
                return;

            if (Preferences.Tooltips && !Preferences.RelevantTooltipsOnly)
                label.tooltip = "Used Memory";
            else
                label.tooltip = string.Empty;

            #if UNITY_5_6_OR_NEWER
            var memory = Profiler.GetRuntimeMemorySizeLong(EnhancedHierarchy.CurrentGameObject);
            #else
            var memory = Profiler.GetRuntimeMemorySize(EnhancedHierarchy.CurrentGameObject);
            #endif

            if (memory == 0)
                return;

            label.text = EditorUtility.FormatBytes(memory);
            m_width = Style.CalcSize(label).x;
        }

        public override void DoGUI(Rect rect) {
            if (m_width <= 0f)
                return;

            rect.xMin += 2f;
            rect.xMax -= 2f;

            using(new GUIColor(Styles.backgroundColorEnabled))
            EditorGUI.LabelField(rect, label, Style);
        }

    }
}
