using System;
using System.Collections.Generic;
using UnityEngine;

namespace EnhancedHierarchy {
    [Serializable]
    public sealed class IconList : List<IconBase>, ISerializationCallbackReceiver {

        [SerializeField]
        private IconData[] data;

        public IconList() { }

        public IconList(IEnumerable<IconBase> collection) : base(collection) { }

        public void OnAfterDeserialize() {
            if (data == null)
                return;

            Clear();

            for (var i = 0; i < data.Length; i++)
                Add(data[i].Icon);
        }

        public void OnBeforeSerialize() {
            data = new IconData[Count];

            for (var i = 0; i < data.Length; i++)
                data[i] = new IconData() { Icon = this[i] };
        }

    }
}
