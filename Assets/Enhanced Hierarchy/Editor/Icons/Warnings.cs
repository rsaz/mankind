using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EnhancedHierarchy.Icons {
    public sealed class Warnings : IconBase {

        private const int MAX_STRING_LEN = 750;
        private const float ICONS_WIDTH = 16f;

        public static StringBuilder goLogs = new StringBuilder(MAX_STRING_LEN);
        public static StringBuilder goWarnings = new StringBuilder(MAX_STRING_LEN);
        public static StringBuilder goErrors = new StringBuilder(MAX_STRING_LEN);
        private static readonly GUIContent tempTooltipContent = new GUIContent();

        private LogEntry log;
        private LogEntry warning;
        private LogEntry error;

        public override string Name { get { return "Logs, Warnings and Errors"; } }
        public override float Width {
            get {
                var result = 0f;

                if (goLogs.Length > 0)
                    result += ICONS_WIDTH;
                if (goWarnings.Length > 0)
                    result += ICONS_WIDTH;
                if (goErrors.Length > 0)
                    result += ICONS_WIDTH;

                return result;
            }
        }

        public override Texture2D PreferencesPreview { get { return Styles.warningIcon; } }

        //public override string PreferencesTooltip { get { return "Some tag for the tooltip here"; } }

        public override void Init() {
            if (!EnhancedHierarchy.IsGameObject)
                return;

            log = null;
            warning = null;
            error = null;

            goLogs.Length = 0;
            goWarnings.Length = 0;
            goErrors.Length = 0;

            var contextEntries = (List<LogEntry>)null;
            var components = EnhancedHierarchy.Components;

            for (var i = 0; i < components.Count; i++)
                if (!components[i])
                    goWarnings.AppendLine("Missing MonoBehaviour\n");

            foreach (var entry in LogEntry.compileEntries
                    .Where(entry => entry.ClassType != null)
                    .Where(entry => EnhancedHierarchy.Components
                        .Any(comp => comp && (comp.GetType() == entry.ClassType || comp.GetType().IsAssignableFrom(entry.ClassType))))) {

                var isWarning = entry.HasMode(EntryMode.ScriptCompileWarning | EntryMode.AssetImportWarning);

                if (goWarnings.Length < MAX_STRING_LEN && isWarning)
                    goWarnings.AppendLine(entry.ToString());

                else if (goErrors.Length < MAX_STRING_LEN && !isWarning)
                    goErrors.AppendLine(entry.ToString());

                if (isWarning && warning == null && !string.IsNullOrEmpty(entry.File))
                    warning = entry;
                if (!isWarning && error == null && !string.IsNullOrEmpty(entry.File))
                    error = entry;

            }

            if (LogEntry.gameObjectEntries.TryGetValue(EnhancedHierarchy.CurrentGameObject, out contextEntries))
                for (var i = 0; i < contextEntries.Count; i++) {

                    var entry = contextEntries[i];
                    var isLog = entry.HasMode(EntryMode.ScriptingLog);
                    var isWarning = entry.HasMode(EntryMode.ScriptingWarning);
                    var isError = entry.HasMode(EntryMode.ScriptingError | EntryMode.ScriptingException | EntryMode.ScriptingAssertion);

                    if (isLog && goLogs.Length < MAX_STRING_LEN)
                        goLogs.AppendLine(entry.ToString());

                    else if (isWarning && goWarnings.Length < MAX_STRING_LEN)
                        goWarnings.AppendLine(entry.ToString());

                    else if (isError && goErrors.Length < MAX_STRING_LEN)
                        goErrors.AppendLine(entry.ToString());

                    if (isLog && log == null && !string.IsNullOrEmpty(entry.File))
                        log = entry;
                    if (isWarning && warning == null && !string.IsNullOrEmpty(entry.File))
                        warning = entry;
                    if (isError && error == null && !string.IsNullOrEmpty(entry.File))
                        error = entry;

                }

        }

        public override void DoGUI(Rect rect) {
            if ((!EnhancedHierarchy.IsRepaintEvent && !Preferences.OpenScriptsOfLogs) || !EnhancedHierarchy.IsGameObject)
                return;

            rect.xMax = rect.xMin + 17f;
            rect.yMax += 1f;

            DoSingleGUI(ref rect, goLogs, Styles.infoIcon, log);
            DoSingleGUI(ref rect, goWarnings, Styles.warningIcon, warning);
            DoSingleGUI(ref rect, goErrors, Styles.errorIcon, error);

        }

        private void DoSingleGUI(ref Rect rect, StringBuilder str, Texture2D icon, LogEntry entry) {
            if (str.Length == 0)
                return;

            if (Utility.ShouldCalculateTooltipAt(rect))
                tempTooltipContent.tooltip = Preferences.Tooltips ? str.ToString().TrimEnd('\n', '\r') : string.Empty;

            tempTooltipContent.image = icon;

            if (GUI.Button(rect, tempTooltipContent, Styles.iconButton))
                if (entry != null)
                    entry.OpenToEdit();

            rect.x += ICONS_WIDTH;
        }

    }
}
