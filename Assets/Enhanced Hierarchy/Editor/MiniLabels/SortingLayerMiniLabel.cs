using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace EnhancedHierarchy {
    public class SortingLayerMiniLabel : MiniLabelProvider {

        private const string DEFAULT_SORTING_LAYER = "Default";

        private string layerName;
        private int sortingOrder;

        public override void FillContent(GUIContent content) {

            var sortingGroup = EnhancedHierarchy.Components.FirstOrDefault(c => c is SortingGroup)as SortingGroup;
            var spriteRenderer = EnhancedHierarchy.Components.FirstOrDefault(c => c is SpriteRenderer)as SpriteRenderer;
            var particleSystem = EnhancedHierarchy.Components.FirstOrDefault(c => c is ParticleSystemRenderer)as ParticleSystemRenderer;

            Type comp = null;
            var hasSortingLayer = true;

            // Gambiarra ahead
            if (sortingGroup) {
                layerName = sortingGroup.sortingLayerName;
                sortingOrder = sortingGroup.sortingOrder;
                comp = sortingGroup.GetType();
            } else if (spriteRenderer) {
                layerName = spriteRenderer.sortingLayerName;
                sortingOrder = spriteRenderer.sortingOrder;
                comp = spriteRenderer.GetType();
            } else if (particleSystem) {
                layerName = particleSystem.sortingLayerName;
                sortingOrder = particleSystem.sortingOrder;
                comp = typeof(ParticleSystem);
            } else {
                hasSortingLayer = false;
            }

            content.text = hasSortingLayer ?
                string.Format("{0}:{1}", layerName, sortingOrder) :
                string.Empty;

            content.tooltip = comp != null && Preferences.Tooltips ?
                string.Format("Sorting layer from {0}", comp.Name) :
                string.Empty;

            // content.image = AssetPreview.GetMiniTypeThumbnail(comp);
        }

        public override bool Faded() {
            return layerName == DEFAULT_SORTING_LAYER && sortingOrder == 0;
        }

        public override void OnClick() {

        }

    }
}
