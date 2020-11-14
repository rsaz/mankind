using System;
#if HIERARCHY_PROFILING
using System.Diagnostics;
using System.Text;
using UnityEngine.Profiling;
#endif
using Object = UnityEngine.Object;

namespace EnhancedHierarchy {
    /// <summary>
    /// Prevents wrong profiler samples count.
    /// Very useful for things other than Enhanced Hierarchy, Unity could implement this on its API, just saying :).
    /// </summary>
    public sealed class ProfilerSample : IDisposable {

        #if HIERARCHY_PROFILING
        private static readonly StringBuilder name = new StringBuilder(150);
        #endif

        private ProfilerSample(string name, Object targetObject) {
            #if HIERARCHY_PROFILING
            if (!targetObject)
                Profiler.BeginSample(name);
            else
                Profiler.BeginSample(name, targetObject);
            #endif
        }

        public static ProfilerSample Get() {
            #if HIERARCHY_PROFILING
            Profiler.BeginSample("Getting Stack Frame");

            var stack = new StackFrame(1, false);

            name.Length = 0;
            name.Append(stack.GetMethod().DeclaringType.Name);
            name.Append(".");
            name.Append(stack.GetMethod().Name);

            Profiler.EndSample();

            return Get(name.ToString(), null);
            #else
            return null;
            #endif
        }

        public static ProfilerSample Get(string name, Object targetObject = null) {
            #if HIERARCHY_PROFILING
            return new ProfilerSample(name, targetObject);
            #else
            return null;
            #endif
        }

        public void Dispose() {
            #if HIERARCHY_PROFILING
            Profiler.EndSample();
            #endif
        }

    }
}
