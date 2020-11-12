using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EnhancedHierarchy {
    public static partial class EnhancedHierarchy {

        public const int UNLAYERED = 0;
        public const string UNTAGGED = "Untagged";
        public const float ALPHA_THRESHOLD = 0.01f;

        private static readonly GUIContent trailingContent = new GUIContent("...");

        public static string GameObjectName { get; private set; }
        public static string GameObjectTag { get; private set; }
        public static bool IsFirstVisible { get; private set; }
        public static bool IsRepaintEvent { get; private set; }
        public static bool IsGameObject { get; private set; }
        public static bool HasTag { get; private set; }
        public static bool HasLayer { get; private set; }
        public static float LeftIconsWidth { get; private set; }
        public static float RightIconsWidth { get; private set; }
        public static float LabelSize { get; private set; }
        public static Rect RawRect { get; private set; }
        public static Rect FullSizeRect { get; private set; }
        public static Rect FinalRect { get; private set; }
        public static Rect SelectionRect { get; private set; }
        public static Rect LabelOnlyRect { get; private set; }
        public static Color CurrentColor { get; private set; }
        public static Vector2 SelectionStart { get; private set; }
        public static GUIStyle CurrentStyle { get; private set; }
        public static GameObject CurrentGameObject { get; private set; }
        public static List<Object> DragSelection { get; private set; }
        public static readonly List<Component> Components = new List<Component>(100);
        public static EventType LastEventType { get; private set; }

        public static void SetItemInformation(int id, Rect rect) {
            if (!Preferences.Enabled)
                return;

            using(ProfilerSample.Get("Enhanced Hierarchy"))
            using(ProfilerSample.Get())
            try {
                CurrentGameObject = EditorUtility.InstanceIDToObject(id)as GameObject;

                IsGameObject = CurrentGameObject;
                IsRepaintEvent = Event.current.type == EventType.Repaint;
                IsFirstVisible = Event.current.type != LastEventType;
                LastEventType = Event.current.type;

                if (IsGameObject) {
                    GameObjectName = CurrentGameObject.name;

                    try {
                        GameObjectTag = CurrentGameObject.tag;
                    } catch { // I couldn't reproduce this, but it can happen
                        if (Preferences.DebugEnabled)
                            Debug.LogWarning("Invalid gameobject tag", CurrentGameObject);
                        GameObjectTag = "Untagged";
                    }

                    LabelSize = EditorStyles.label.CalcSize(Utility.GetTempGUIContent(GameObjectName)).x;
                    LabelSize += Reflected.IconWidth + 5f; // Icon size 
                    var labelOnlyRect = rect;
                    labelOnlyRect.xMax = labelOnlyRect.xMin + LabelSize;
                    LabelOnlyRect = labelOnlyRect;
                    HasTag = !CurrentGameObject.CompareTag(UNTAGGED) || !Preferences.HideDefaultTag;
                    HasLayer = CurrentGameObject.layer != UNLAYERED || !Preferences.HideDefaultLayer;
                    CurrentStyle = Utility.GetHierarchyLabelStyle(CurrentGameObject);
                    CurrentColor = CurrentStyle.normal.textColor;
                    CurrentGameObject.GetComponents(Components);
                }

                if (IsFirstVisible)
                    FinalRect = RawRect;

                RawRect = rect;
                rect.xMin = 0f;
                //rect.xMax = EditorGUIUtility.currentViewWidth;
                FullSizeRect = rect;
            } catch (Exception e) {
                Utility.LogException(e);
            }
        }

    }
}
