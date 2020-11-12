using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EnhancedHierarchy {
    /// <summary>
    /// Log Entries from the console, to check if a game object has any errors or warnings.
    /// </summary>
    public sealed class LogEntry {

        private const double UPDATE_FREQUENCY = 0.75; // Every 750ms

        private static readonly Type logEntriesType;
        private static readonly Type logEntryType;

        public int RowIndex { get; private set; }
        public string Condition { get; private set; }
        public int ErrorNum { get; private set; }
        public string File { get; private set; }
        public int Line { get; private set; }
        public int Column { get; private set; }
        public EntryMode Mode { get; private set; }
        public int InstanceID { get; private set; }
        public int Identifier { get; private set; }
        public Object ObjectReference { get; private set; }
        public MonoScript Script { get; private set; }
        public Type ClassType { get; private set; }

        public static Dictionary<GameObject, List<LogEntry>> gameObjectEntries = new Dictionary<GameObject, List<LogEntry>>(100);
        public static List<LogEntry> compileEntries = new List<LogEntry>(100);

        private static int lastCount;
        private static bool entriesDirty;
        private static bool lastCompileFailedState;
        private static double lastUpdatedTime;
        private static readonly Icons.Warnings warnings = new Icons.Warnings();

        static LogEntry() {
            try {
                logEntriesType = ReflectionHelper.FindType("UnityEditorInternal.LogEntries");
                logEntryType = ReflectionHelper.FindType("UnityEditorInternal.LogEntry");

                if (logEntriesType == null)
                    logEntriesType = ReflectionHelper.FindType("UnityEditor.LogEntries");
                if (logEntryType == null)
                    logEntryType = ReflectionHelper.FindType("UnityEditor.LogEntry");

                ReloadReferences();
            } catch (Exception e) {
                Debug.LogException(e);
                Preferences.ForceDisableButton(new Icons.Warnings());
            }

            Application.logMessageReceived += (logString, stackTrace, type) => MarkEntriesDirty();

            EditorApplication.update += () => {
                try {
                    #if UNITY_2017_1_OR_NEWER
                    if (!entriesDirty && EditorUtility.scriptCompilationFailed != lastCompileFailedState) {
                        lastCompileFailedState = EditorUtility.scriptCompilationFailed;
                        MarkEntriesDirty();
                    }
                    #endif

                    if (EditorApplication.timeSinceStartup - lastUpdatedTime > UPDATE_FREQUENCY) {

                        if (!entriesDirty) {
                            var currentCount = GetLogCount();
                            if (lastCount > currentCount) { // Console possibly cleared
                                if (Preferences.DebugEnabled)
                                    Debug.Log("Detected console clear");
                                MarkEntriesDirty();
                            }
                            lastCount = currentCount;
                        }

                        if (entriesDirty)
                            ReloadReferences();

                    }
                } catch (Exception e) {
                    Debug.LogException(e);
                    Preferences.ForceDisableButton(new Icons.Warnings());
                }
            };
        }

        private LogEntry(object nativeEntry, int rowIndex) {
            RowIndex = rowIndex;

            if (nativeEntry.HasField("condition"))
                Condition = nativeEntry.GetInstanceField<string>("condition");
            else if (nativeEntry.HasField("message"))
                Condition = nativeEntry.GetInstanceField<string>("message");
            else
                throw new MissingFieldException("LogEntry doesn't have a message field");

            if (nativeEntry.HasField("errorNum"))
                ErrorNum = nativeEntry.GetInstanceField<int>("errorNum");

            File = nativeEntry.GetInstanceField<string>("file");
            Line = nativeEntry.GetInstanceField<int>("line");

            if (nativeEntry.HasField("column"))
                Column = nativeEntry.GetInstanceField<int>("column");

            Mode = nativeEntry.GetInstanceField<EntryMode>("mode");
            InstanceID = nativeEntry.GetInstanceField<int>("instanceID");
            Identifier = nativeEntry.GetInstanceField<int>("identifier");

            if (InstanceID != 0)
                ObjectReference = EditorUtility.InstanceIDToObject(InstanceID);

            if (ObjectReference)
                Script = ObjectReference as MonoScript;

            if (Script)
                ClassType = Script.GetClass();
        }

        public static void MarkEntriesDirty() {
            if (!entriesDirty && Preferences.Enabled && Preferences.IsButtonEnabled(warnings))
                entriesDirty = true;
        }

        private static void ReloadReferences() {

            if (Preferences.DebugEnabled)
                Debug.Log("Reloading Logs References");

            gameObjectEntries.Clear();
            compileEntries.Clear();

            try {
                var count = logEntriesType.InvokeStaticMethod<int>("StartGettingEntries");
                var nativeEntry = Activator.CreateInstance(logEntryType);

                for (var i = 0; i < count; i++) {
                    logEntriesType.InvokeStaticMethod("GetEntryInternal", i, nativeEntry);

                    var proxyEntry = new LogEntry(nativeEntry, i);
                    var go = proxyEntry.ObjectReference as GameObject;

                    if (proxyEntry.ObjectReference && !go) {
                        var component = proxyEntry.ObjectReference as Component;

                        if (component)
                            go = component.gameObject;
                    }

                    // if(entry.HasMode(EntryMode.ScriptCompileError | EntryMode.ScriptCompileWarning | EntryMode.AssetImportWarning) && entry.ClassType != null)
                    // if(!referencedComponents.Any(e => e.ClassType == entry.ClassType))
                    if (proxyEntry.ClassType != null)
                        compileEntries.Add(proxyEntry);

                    if (go)
                        if (gameObjectEntries.ContainsKey(go))
                            gameObjectEntries[go].Add(proxyEntry);
                        else
                            gameObjectEntries.Add(go, new List<LogEntry>() { proxyEntry });
                }

                EditorApplication.RepaintHierarchyWindow();
            } catch (Exception e) {
                Debug.LogException(e);
                Preferences.ForceDisableButton(new Icons.Warnings());
            } finally {
                entriesDirty = false;
                lastUpdatedTime = EditorApplication.timeSinceStartup;
                logEntriesType.InvokeStaticMethod("EndGettingEntries");
            }
        }

        public bool HasMode(EntryMode toCheck) {
            return (Mode & toCheck) != 0;
        }

        public void OpenToEdit() {
            logEntriesType.InvokeStaticMethod("RowGotDoubleClicked", RowIndex);
        }

        private static int GetLogCount() {
            return logEntriesType.InvokeStaticMethod<int>("GetCount");
        }

        public override string ToString() {
            return Condition;
        }

    }
}
