using System;
using System.Collections.Generic;
using System.Linq;
using EnhancedHierarchy.Icons;
using UnityEditor;
using UnityEngine;

namespace EnhancedHierarchy {

    public enum IconPosition {
        AfterObjectName = 1,
        BeforeObjectName = 2,
        RightMost = 4,
        SafeArea = AfterObjectName | RightMost,
        All = SafeArea | BeforeObjectName
    }

    public abstract class IconBase {

        private const float DEFAULT_WIDTH = 16f;

        public static None none = new None();

        public virtual string Name { get { return GetType().Name; } }
        public virtual float Width { get { return DEFAULT_WIDTH; } } // May be called multiple times in the same frame for the same object

        public virtual IconPosition Side { get { return IconPosition.SafeArea; } }

        public virtual Texture2D PreferencesPreview { get { return null; } }

        public virtual string PreferencesTooltip { get { return null; } }

        public static IconBase[] AllLeftIcons { get; private set; }
        public static IconBase[] AllRightIcons { get; private set; }
        public static IconBase[] AllLeftOfNameIcons { get; private set; }

        static IconBase() {
            var baseType = typeof(IconBase);

            icons = baseType.Assembly.GetTypes()
                .Where(t => t != baseType && baseType.IsAssignableFrom(t))
                .Select(t => (IconBase)Activator.CreateInstance(t))
                .ToDictionary(t => t.Name);

            AllLeftIcons = icons.Select(i => i.Value).Where(i => (i.Side & IconPosition.AfterObjectName) != 0).ToArray();
            AllRightIcons = icons.Select(i => i.Value).Where(i => (i.Side & IconPosition.RightMost) != 0).ToArray();
            AllLeftOfNameIcons = icons.Select(i => i.Value).Where(i => (i.Side & IconPosition.BeforeObjectName) != 0).ToArray();
        }

        public virtual void Init() { } // Guaranteed to be called only once for each obj in every frame before any DoGUI() and get Width calls
        public abstract void DoGUI(Rect rect);

        private static readonly Dictionary<string, IconBase> icons = new Dictionary<string, IconBase>();

        protected static ChildrenChangeMode AskChangeModeIfNecessary(List<GameObject> objs, ChildrenChangeMode reference, string title, string message) {
            var controlPressed = Event.current.control || Event.current.command;

            switch (reference) {
                case ChildrenChangeMode.ObjectOnly:
                    return controlPressed ? ChildrenChangeMode.ObjectAndChildren : ChildrenChangeMode.ObjectOnly;

                case ChildrenChangeMode.ObjectAndChildren:
                    return controlPressed ? ChildrenChangeMode.ObjectOnly : ChildrenChangeMode.ObjectAndChildren;

                default:
                    for (var i = 0; i < objs.Count; i++)
                        if (objs[i] && objs[i].transform.childCount > 0)
                            try {
                                return (ChildrenChangeMode)EditorUtility.DisplayDialogComplex(title, message, "Yes, change children", "No, this object only", "Cancel");
                            } finally {
                                //Unity bug, DisplayDialogComplex makes the unity partially lose focus
                                if (EditorWindow.focusedWindow)
                                    EditorWindow.focusedWindow.Focus();
                            }

                    return ChildrenChangeMode.ObjectOnly;
            }
        }

        protected static List<GameObject> GetSelectedObjectsAndCurrent() {

            if (!Preferences.ChangeAllSelected || Selection.gameObjects.Length < 2)
                return EnhancedHierarchy.CurrentGameObject ?
                    new List<GameObject> { EnhancedHierarchy.CurrentGameObject } :
                    new List<GameObject>();

            return Selection.gameObjects
                .Where(obj => !EditorUtility.IsPersistent(obj)) // Makes sure the object is part of the scene and not the project
                .Union(EnhancedHierarchy.CurrentGameObject ? new [] { EnhancedHierarchy.CurrentGameObject } : new GameObject[0])
                .Distinct()
                .ToList();
        }

        public static bool operator ==(IconBase left, IconBase right) {
            if (ReferenceEquals(left, right))
                return true;

            if (ReferenceEquals(left, null))
                return false;

            if (ReferenceEquals(right, null))
                return false;

            return left.Name == right.Name;
        }

        public static bool operator !=(IconBase left, IconBase right) {
            return !(left == right);
        }

        public override string ToString() {
            return Name;
        }

        public override int GetHashCode() {
            return Name.GetHashCode();
        }

        public override bool Equals(object obj) {
            return obj as IconBase == this;
        }

        public static implicit operator IconBase(string name) {
            try { return icons[name]; } catch { return none; }
        }

        public static implicit operator string(IconBase icon) {
            return icon.ToString();
        }

    }

}
