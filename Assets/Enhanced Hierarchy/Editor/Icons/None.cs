using UnityEngine;

namespace EnhancedHierarchy.Icons {
    public sealed class None : IconBase {
        public override float Width { get { return 0f; } }
        public override string Name { get { return "None"; } }
        public override IconPosition Side { get { return IconPosition.All; } }
        public override void DoGUI(Rect rect) { }
    }
}
