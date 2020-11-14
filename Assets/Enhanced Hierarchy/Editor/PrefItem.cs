using System;
using UnityEditor;
using UnityEngine;

namespace EnhancedHierarchy {
    /// <summary>
    /// Generic preference item interface.
    /// </summary>
    public interface IPrefItem {

        bool Drawing { get; }
        object Value { get; set; }

        GUIContent Label { get; }

        //void DoGUI();

        GUIEnabled GetEnabledScope();
        GUIEnabled GetEnabledScope(bool enabled);
        GUIFade GetFadeScope(bool enabled);
    }

    /// <summary>
    /// Generic preference item.
    /// </summary>
    [Serializable]
    public sealed class PrefItem<T> : IPrefItem {

        [Serializable]
        private struct Wrapper {
            [SerializeField]
            public T value;
        }

        private const string KEY_PREFIX = "EH.";

        private string key;
        private Wrapper wrapper;
        private T defaultValue;

        private readonly GUIFade fade;

        public GUIContent Label { get; private set; }

        public bool Drawing { get { return fade.Visible; } }

        public T DefaultValue {
            get { return defaultValue; }
            set { SetDefaultValue(value); }
        }

        public T Value {
            get { return wrapper.value; }
            set { SetValue(value, false); }
        }

        private bool UsingDefaultValue { get { return !EditorPrefs.HasKey(key); } }

        object IPrefItem.Value {
            get { return Value; }
            set { Value = (T)value; }
        }

        public PrefItem(string key, T defaultValue, string text = "", string tooltip = "") {
            this.key = KEY_PREFIX + key;
            this.defaultValue = defaultValue;

            Label = new GUIContent(text, tooltip);
            fade = new GUIFade();

            Preferences.contents.Add(Label);
            Preferences.onResetPreferences += ResetValue;

            if (UsingDefaultValue)
                wrapper.value = Clone(defaultValue);
            else
                LoadValue();
        }

        public void SetDefaultValue(T newDefault) {
            if (UsingDefaultValue)
                wrapper.value = Clone(newDefault);
            defaultValue = newDefault;
        }

        private void LoadValue() {
            try {
                if (!EditorPrefs.HasKey(key))
                    return;

                var json = EditorPrefs.GetString(key);

                // if(Preferences.DebugEnabled)
                //    Debug.LogFormat("Loading preference {0}: {1}", key, json);

                wrapper = JsonUtility.FromJson<Wrapper>(json);
            } catch (Exception e) {
                Debug.LogWarningFormat("Failed to load preference item \"{0}\", using default value: {1}", key, defaultValue);
                Debug.LogException(e);
                ResetValue();
            }
        }

        private void SetValue(T newValue, bool forceSave) {
            try {
                if (Value != null && Value.Equals(newValue) && !forceSave)
                    return;

                wrapper.value = newValue;

                var json = JsonUtility.ToJson(wrapper, Preferences.DebugEnabled);

                // if(Preferences.DebugEnabled)
                //    Debug.LogFormat("Saving preference {0}: {1}", key, json);

                EditorPrefs.SetString(key, json);
            } catch (Exception e) {
                Debug.LogWarningFormat("Failed to save {0}: {1}", key, e);
                Debug.LogException(e);
            } finally {
                wrapper.value = newValue;
            }
        }

        private void ResetValue() {
            if (UsingDefaultValue)
                return;

            if (Preferences.DebugEnabled)
                Debug.LogFormat("Deleted preference {0}", key);

            wrapper.value = Clone(defaultValue);
            EditorPrefs.DeleteKey(key);
        }

        public void ForceSave() {
            SetValue(wrapper.value, true);
        }

        private T Clone(T other) {
            if (typeof(T).IsValueType)
                return other;

            var wrapper = new Wrapper() { value = other };
            var json = JsonUtility.ToJson(wrapper, Preferences.DebugEnabled);
            var clonnedWrapper = JsonUtility.FromJson<Wrapper>(json);

            // if(Preferences.DebugEnabled)
            //     Debug.LogFormat("Clone of {0}: {1}", key, json);

            return clonnedWrapper.value;
        }

        public GUIEnabled GetEnabledScope() {
            return GetEnabledScope(Value.Equals(true));
        }

        public GUIEnabled GetEnabledScope(bool enabled) {
            return new GUIEnabled(enabled);
        }

        public GUIFade GetFadeScope(bool enabled) {
            fade.SetTarget(enabled);
            return fade;
        }

        public static implicit operator T(PrefItem<T> pb) {
            if (pb == null) {
                Debug.LogError("Cannot get the value of a null PrefItem");
                return default(T);
            }

            return pb.Value;
        }

        public static implicit operator GUIContent(PrefItem<T> pb) {
            if (pb == null) {
                Debug.LogError("Cannot get the content of a null PrefItem");
                return new GUIContent("Null PrefItem");
            }

            return pb.Label;
        }

    }
}
