using System;
using UnityEngine;

namespace EnhancedHierarchy {
    [Serializable]
    public class IconData : ISerializationCallbackReceiver {

        [SerializeField]
        private string name;

        public IconBase Icon { get; set; }

        public void OnAfterDeserialize() {
            Icon = name;
        }

        public void OnBeforeSerialize() {
            if (Icon == null)
                return;

            name = Icon.Name;
        }

    }
}
