using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace EnhancedHierarchy {

    public enum TintMode {
        Flat = 0,
        GradientRightToLeft = 1,
        GradientLeftToRight = 2,
    }

    /// <summary>
    /// Per layer color setting.
    /// </summary>
    [Serializable]
    public struct LayerColor {

        [SerializeField]
        public int layer;
        [SerializeField]
        public Color color;
        [SerializeField]
        public TintMode mode;

        public LayerColor(int layer) : this(layer, Color.clear) { }

        public LayerColor(int layer, Color color, TintMode mode = TintMode.GradientRightToLeft) {
            this.layer = layer;
            this.color = color;
            this.mode = mode;
        }

        public static implicit operator LayerColor(int layer) {
            return new LayerColor(layer);
        }

        public static bool operator ==(LayerColor left, LayerColor right) {
            return left.layer == right.layer;
        }

        public static bool operator !=(LayerColor left, LayerColor right) {
            return left.layer != right.layer;
        }

        public override bool Equals(object obj) {
            if (!(obj is LayerColor))
                return false;

            return ((LayerColor)obj).layer == layer;
        }

        public override int GetHashCode() {
            return layer.GetHashCode();
        }

    }

    /// <summary>
    /// Save and load hierarchy preferences.
    /// </summary>
    public static partial class Preferences {

        [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
        private class AutoPrefItemAttribute : Attribute {

            public string Key { get; private set; }

            public AutoPrefItemAttribute(string key = null) { Key = key; }

        }

        [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
        private class AutoPrefItemDefaultValueAttribute : Attribute {

            public object DefaultValue { get; private set; }

            public AutoPrefItemDefaultValueAttribute(object defaultValue) { DefaultValue = defaultValue; }

        }

        [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
        private class AutoPrefItemLabelAttribute : Attribute {

            public GUIContent Label { get; private set; }

            public AutoPrefItemLabelAttribute(string label, string tooltip = null) { Label = new GUIContent(label, tooltip); }

        }

        private static Color DefaultOddSortColor { get { return EditorGUIUtility.isProSkin ? new Color(0f, 0f, 0f, 0.1f) : new Color(1f, 1f, 1f, 0.2f); } }
        private static Color DefaultEvenSortColor { get { return EditorGUIUtility.isProSkin ? new Color(0f, 0f, 0f, 0f) : new Color(1f, 1f, 1f, 0f); } }
        private static Color DefaultLineColor { get { return new Color(0f, 0f, 0f, 0.2f); } }
        private static Color DefaultHoverTint { get { return EditorGUIUtility.isProSkin ? new Color(0f, 0f, 0f, 0.2f) : new Color(0.12f, 0.12f, 0.12f, 0.2f); } }

        #region PrefItems
        [AutoPrefItem]
        [AutoPrefItemDefaultValue(true)]
        [AutoPrefItemLabel("Enabled", "Enable or disable the entire plugin, it will be automatically disabled if any error occurs")]
        public static PrefItem<bool> Enabled;

        [AutoPrefItem]
        [AutoPrefItemDefaultValue(0)]
        [AutoPrefItemLabel("Right Margin", "Margin for icons, useful if you have more extensions that also uses hierarchy")]
        public static PrefItem<int> RightMargin;

        [AutoPrefItem]
        [AutoPrefItemDefaultValue(0)]
        [AutoPrefItemLabel("Left Margin", "Margin for icons, useful if you have more extensions that also uses hierarchy")]
        public static PrefItem<int> LeftMargin;

        [AutoPrefItem]
        [AutoPrefItemDefaultValue(14)]
        [AutoPrefItemLabel("Indent", "Indent for labels, useful for thin hierarchies")]
        public static PrefItem<int> Indent;

        [AutoPrefItem]
        [AutoPrefItemDefaultValue(0.8f)]
        [AutoPrefItemLabel("Hierarchy Tree Opacity", "The opacity of the tree view lines connecting child transforms to their parent, useful if you have multiple children inside children")]
        public static PrefItem<float> TreeOpacity;

        [AutoPrefItem]
        [AutoPrefItemDefaultValue(0.5f)]
        [AutoPrefItemLabel("Stem Proportion", "Stem length for hierarchy items that have no children")]
        public static PrefItem<float> TreeStemProportion;

        [AutoPrefItem]
        [AutoPrefItemDefaultValue(true)]
        [AutoPrefItemLabel("Select on Tree", "Select the parent when you click on the tree lines\n\nTHIS MAY AFFECT PERFORMANCE")]
        public static PrefItem<bool> SelectOnTree;

        [AutoPrefItem]
        [AutoPrefItemDefaultValue(true)]
        [AutoPrefItemLabel("Tooltips", "Shows tooltips, like this one")]
        public static PrefItem<bool> Tooltips;

        [AutoPrefItem]
        [AutoPrefItemDefaultValue(true)]
        [AutoPrefItemLabel("Relevant Tooltips Only", "Hide tooltips that have static texts")]
        public static PrefItem<bool> RelevantTooltipsOnly;

        [AutoPrefItem]
        [AutoPrefItemDefaultValue(true)]
        [AutoPrefItemLabel("Enhanced selection", "Allow selecting GameObjects by dragging over them with right mouse button")]
        public static PrefItem<bool> EnhancedSelection;

        [AutoPrefItem]
        [AutoPrefItemLabel("Highlight tint", "Tint the item under the mouse cursor")]
        public static PrefItem<Color> HoverTintColor;

        [AutoPrefItem]
        [AutoPrefItemDefaultValue(false)]
        [AutoPrefItemLabel("Hide native icon", "Hide the native icon on the left side of the name, introducted in Unity 2018.3")]
        public static PrefItem<bool> DisableNativeIcon;

        [AutoPrefItem]
        [AutoPrefItemDefaultValue(true)]
        [AutoPrefItemLabel("Trailing", "Append ... when names are bigger than the view area")]
        public static PrefItem<bool> Trailing;

        [AutoPrefItem]
        [AutoPrefItemDefaultValue(true)]
        [AutoPrefItemLabel("Select locked objects", "Allow selecting objects that are locked")]
        public static PrefItem<bool> AllowSelectingLockedObjects;

        [AutoPrefItem]
        [AutoPrefItemDefaultValue(false)]
        [AutoPrefItemLabel("Pick locked objects", "Allow picking objects that are locked on scene view\nObjects locked before you change this option will not be affected\nRequires Unity 2019.3 or newer")]
        public static PrefItem<bool> AllowPickingLockedObjects;

        [AutoPrefItem]
        [AutoPrefItemDefaultValue(true)]
        [AutoPrefItemLabel("Change all selected", "This will make the enable, lock, layer, tag and static buttons affect all selected objects in the hierarchy")]
        public static PrefItem<bool> ChangeAllSelected;

        [AutoPrefItem]
        [AutoPrefItemDefaultValue(true)]
        [AutoPrefItemLabel("Left side button at leftmost", "Put the left button to the leftmost side of the hierarchy, if disabled it will be next to the game object name")]
        public static PrefItem<bool> LeftmostButton;

        [AutoPrefItem]
        [AutoPrefItemDefaultValue(true)]
        [AutoPrefItemLabel("Open scripts of logs", "Clicking on warnings, logs or errors will open the script to edit in your IDE or text editor\n\nMAY AFFECT PERFORMANCE")]
        public static PrefItem<bool> OpenScriptsOfLogs;

        [AutoPrefItem]
        [AutoPrefItemDefaultValue(false)]
        [AutoPrefItemLabel("Replace default child toggle", "Replace the default toggle for expanding children to a new one that shows the children count")]
        public static PrefItem<bool> NumericChildExpand;

        [AutoPrefItem]
        [AutoPrefItemDefaultValue(true)]
        [AutoPrefItemLabel("Smaller font", "Use a smaller font on the minilabel for narrow hierarchies")]
        public static PrefItem<bool> SmallerMiniLabel;

        [AutoPrefItem]
        [AutoPrefItemDefaultValue(15)]
        [AutoPrefItemLabel("Icons Size", "The size of the icons in pixels")]
        public static PrefItem<int> IconsSize;

        [AutoPrefItem]
        [AutoPrefItemDefaultValue(true)]
        [AutoPrefItemLabel("Centralize when possible", "Centralize minilabel when there's only tag or only layer on it")]
        public static PrefItem<bool> CentralizeMiniLabelWhenPossible;

        [AutoPrefItem]
        [AutoPrefItemDefaultValue(true)]
        [AutoPrefItemLabel("Hide \"Untagged\" tag", "Hide default tag on minilabel")]
        public static PrefItem<bool> HideDefaultTag;

        [AutoPrefItem]
        [AutoPrefItemDefaultValue(true)]
        [AutoPrefItemLabel("Hide \"Default\" layer", "Hide default layer on minilabel")]
        public static PrefItem<bool> HideDefaultLayer;

        [AutoPrefItem]
        [AutoPrefItemDefaultValue(false)]
        [AutoPrefItemLabel("Hide default icon", "Hide the default game object icon")]
        public static PrefItem<bool> HideDefaultIcon;

        [AutoPrefItem]
        [AutoPrefItemDefaultValue(1)]
        [AutoPrefItemLabel("Line thickness", "Separator line thickness")]
        public static PrefItem<int> LineSize;

        [AutoPrefItem]
        [AutoPrefItemLabel("Odd row tint", "The tint of odd rows")]
        public static PrefItem<Color> OddRowColor;

        [AutoPrefItem]
        [AutoPrefItemLabel("Even row tint", "The tint of even rows")]
        public static PrefItem<Color> EvenRowColor;

        [AutoPrefItem]
        [AutoPrefItemLabel("Line tint", "The tint of separators line")]
        public static PrefItem<Color> LineColor;

        [AutoPrefItem]
        [AutoPrefItemLabel("Left side button", "The button that will appear in the left side of the hierarchy\nLooks better with \"Hierarchy Tree\" disabled")]
        public static PrefItem<IconData> LeftSideButtonPref;

        [AutoPrefItem]
        [AutoPrefItemLabel("Mini label", "The little label next to the GameObject name")]
        public static PrefItem<int[]> MiniLabels;

        [AutoPrefItem]
        [AutoPrefItemDefaultValue(ChildrenChangeMode.ObjectAndChildren)]
        [AutoPrefItemLabel("Lock", "Which objects will be locked when you click on the lock toggle")]
        public static PrefItem<ChildrenChangeMode> LockAskMode;

        [AutoPrefItem]
        [AutoPrefItemDefaultValue(ChildrenChangeMode.Ask)]
        [AutoPrefItemLabel("Layer", "Which objects will have their layer changed when you click on the layer button or on the mini label")]
        public static PrefItem<ChildrenChangeMode> LayerAskMode;

        [AutoPrefItem]
        [AutoPrefItemDefaultValue(ChildrenChangeMode.ObjectOnly)]
        [AutoPrefItemLabel("Tag", "Which objects will have their tag changed when you click on the tag button or on the mini label")]
        public static PrefItem<ChildrenChangeMode> TagAskMode;

        [AutoPrefItem]
        [AutoPrefItemDefaultValue(ChildrenChangeMode.Ask)]
        [AutoPrefItemLabel("Static", "Which flags will be changed when you click on the static toggle")]
        public static PrefItem<ChildrenChangeMode> StaticAskMode;

        [AutoPrefItem]
        [AutoPrefItemDefaultValue(ChildrenChangeMode.ObjectOnly)]
        [AutoPrefItemLabel("Icon", "Which objects will have their icon changed when you click on the icon button")]
        public static PrefItem<ChildrenChangeMode> IconAskMode;

        [AutoPrefItem]
        [AutoPrefItemLabel("Icons next to the object name", "The icons that appear next to the game object name")]
        public static PrefItem<IconList> LeftIcons;

        [AutoPrefItem]
        [AutoPrefItemLabel("Icons on the rightmost", "The icons that appear to the rightmost of the hierarchy")]
        public static PrefItem<IconList> RightIcons;

        [AutoPrefItem]
        [AutoPrefItemLabel("Per layer row color", "Set a row color for each different layer")]
        public static PrefItem<List<LayerColor>> PerLayerRowColors;
        #endregion

        public static MiniLabelProvider[] miniLabelProviders;

        public static IconBase LeftSideButton {
            get { return LeftSideButtonPref.Value.Icon; }
            set {
                LeftSideButtonPref.Value.Icon = value;
                LeftSideButtonPref.ForceSave();
            }
        }

        public static bool ProfilingEnabled {
            get {
                #if HIERARCHY_PROFILING
                return true;
                #else
                return false;
                #endif
            }
        }

        public static bool DebugEnabled {
            get {
                #if HIERARCHY_DEBUG
                return true;
                #else
                return false;
                #endif
            }
        }

        public static bool MiniLabelTagEnabled {
            get { return miniLabelProviders.Any(ml => ml is TagMiniLabel); }
        }

        public static bool MiniLabelLayerEnabled {
            get { return miniLabelProviders.Any(ml => ml is LayerMiniLabel); }
        }

        public static bool EnhancedSelectionSupported {
            get { return Application.platform == RuntimePlatform.WindowsEditor; }
        }

        static Preferences() {
            InitializePreferences();

            Enabled.Label.text = string.Format("Enabled ({0}+H)", Utility.CtrlKey);

            #if UNITY_2018_3_OR_NEWER
            LeftSideButtonPref.DefaultValue = new IconData() { Icon = new Icons.None() };
            #else
            LeftSideButtonPref.DefaultValue = new IconData() { Icon = new Icons.GameObjectIcon() };
            #endif

            LineColor.DefaultValue = DefaultLineColor;
            OddRowColor.DefaultValue = DefaultOddSortColor;
            EvenRowColor.DefaultValue = DefaultEvenSortColor;
            HoverTintColor.DefaultValue = DefaultHoverTint;

            var defaultLeftIcons = new IconList { new Icons.MonoBehaviourIcon(), new Icons.Warnings(), new Icons.SoundIcon() };
            var defaultRightIcons = new IconList { new Icons.Active(), new Icons.Lock(), new Icons.Static(), new Icons.PrefabApply() };
            var defaultLayerColors = new List<LayerColor> { new LayerColor(5, new Color(0f, 0f, 1f, 0.3019608f)) };

            LeftIcons.DefaultValue = defaultLeftIcons;
            RightIcons.DefaultValue = defaultRightIcons;
            PerLayerRowColors.DefaultValue = defaultLayerColors;
            MiniLabels.DefaultValue = new [] {
                Array.IndexOf(MiniLabelProvider.MiniLabelsTypes, typeof(LayerMiniLabel)),
                Array.IndexOf(MiniLabelProvider.MiniLabelsTypes, typeof(TagMiniLabel))
            };

            minilabelsNames = MiniLabelProvider.MiniLabelsTypes
                .Select(ml => ml == null? "None": ObjectNames.NicifyVariableName(ml.Name.Replace("MiniLabel", "")))
                .ToArray();

            leftIconsList = GenerateReordableList(LeftIcons);
            rightIconsList = GenerateReordableList(RightIcons);

            leftIconsList.onAddDropdownCallback = (rect, newList) => LeftIconsMenu.DropDown(rect);
            rightIconsList.onAddDropdownCallback = (rect, newList) => RightIconsMenu.DropDown(rect);

            rowColorsList = GenerateReordableList(PerLayerRowColors);
            rowColorsList.onAddDropdownCallback = (rect, newList) => RowColorsMenu.DropDown(rect);

            rowColorsList.drawElementCallback = (rect, index, focused, active) => {
                GUI.changed = false;

                rect.xMin -= EditorGUI.indentLevel * 16f;
                PerLayerRowColors.Value[index] = LayerColorField(rect, PerLayerRowColors.Value[index]);

                if (GUI.changed)
                    PerLayerRowColors.ForceSave();
            };

            RecreateMiniLabelProviders();
        }

        public static void RecreateMiniLabelProviders() {
            miniLabelProviders = MiniLabels.Value
                .Select(ml => MiniLabelProvider.MiniLabelsTypes.ElementAtOrDefault(ml))
                .Where(ml => ml != null)
                .Select(ml => Activator.CreateInstance(ml)as MiniLabelProvider)
                .ToArray();
        }

        public static bool IsButtonEnabled(IconBase button) {
            if (button == null)
                return false;

            if (LeftSideButton == button)
                return true;

            return RightIcons.Value.Contains(button) || LeftIcons.Value.Contains(button);
        }

        public static void ForceDisableButton(IconBase button) {
            if (button == null)
                Debug.LogError("Removing null button");
            else
                Debug.LogWarning("Disabling \"" + button.Name + "\", most likely because it threw an exception");

            if (LeftSideButton == button)
                LeftSideButton = IconBase.none;

            RightIcons.Value.Remove(button);
            LeftIcons.Value.Remove(button);

            RightIcons.ForceSave();
            LeftIcons.ForceSave();
        }

        private static void InitializePreferences() {
            var type = typeof(Preferences);
            var members = type.GetMembers(ReflectionHelper.FULL_BINDING);

            foreach (var member in members)
                try {
                    if (member == null)
                        continue;

                    var prefItemType = (Type)null;
                    var prop = member as PropertyInfo;
                    var field = member as FieldInfo;

                    switch (member.MemberType) {
                        case MemberTypes.Field:
                            if (typeof(IPrefItem).IsAssignableFrom(field.FieldType))
                                prefItemType = field.FieldType;
                            else
                                continue;
                            break;

                        case MemberTypes.Property:
                            if (typeof(IPrefItem).IsAssignableFrom(prop.PropertyType))
                                prefItemType = prop.PropertyType;
                            else
                                continue;
                            break;

                        default:
                            continue;
                    }

                    var keyAttribute = (AutoPrefItemAttribute)member.GetCustomAttributes(typeof(AutoPrefItemAttribute), true).FirstOrDefault();
                    var labelAttribute = (AutoPrefItemLabelAttribute)member.GetCustomAttributes(typeof(AutoPrefItemLabelAttribute), true).FirstOrDefault();
                    var defaultValueAttribute = (AutoPrefItemDefaultValueAttribute)member.GetCustomAttributes(typeof(AutoPrefItemDefaultValueAttribute), true).FirstOrDefault();

                    var key = member.Name;
                    var defaultValue = (object)null;
                    var label = new GUIContent(key);

                    //var savedValueType = prefItemType.GetGenericArguments()[0];

                    if (keyAttribute == null)
                        continue;

                    if (!string.IsNullOrEmpty(keyAttribute.Key))
                        key = keyAttribute.Key;

                    if (labelAttribute != null)
                        label = labelAttribute.Label;

                    if (defaultValueAttribute != null)
                        defaultValue = defaultValueAttribute.DefaultValue;
                    //else if(savedValueType.IsValueType)
                    //    defaultValue = Activator.CreateInstance(savedValueType);

                    var prefItem = Activator.CreateInstance(prefItemType, key, defaultValue, label.text, label.tooltip);

                    switch (member.MemberType) {
                        case MemberTypes.Field:
                            field.SetValue(null, prefItem);
                            break;

                        case MemberTypes.Property:
                            prop.SetValue(null, prefItem, null);
                            break;
                    }

                } catch (Exception e) {
                    Debug.LogException(e);
                }
        }
    }
}
