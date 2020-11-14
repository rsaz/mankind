using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace EnhancedHierarchy {
    /// <summary>
    /// Class containing method extensions for getting private and internal members.
    /// </summary>
    public static partial class ReflectionHelper {

        public const BindingFlags FULL_BINDING = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
        public const BindingFlags INSTANCE_BINDING = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
        public const BindingFlags STATIC_BINDING = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;

        private static Assembly[] cachedAssemblies;
        private static Dictionary<string, Type> cachedTypes;

        public static Type FindType(string name) {
            Type result;

            if (cachedTypes == null)
                cachedTypes = new Dictionary<string, Type>();

            if (cachedTypes.TryGetValue(name, out result))
                return result;

            result = FindTypeInAssembly(name, typeof(Editor).Assembly);

            if (result == null) {
                if (cachedAssemblies == null)
                    cachedAssemblies = AppDomain.CurrentDomain.GetAssemblies();

                for (var i = 0; i < cachedAssemblies.Length; i++) {
                    result = FindTypeInAssembly(name, cachedAssemblies[i]);

                    if (result != null)
                        break;
                }
            }

            if (Preferences.DebugEnabled)
                if (result == null)
                    Debug.LogFormat("Cache fault for \"{0}\", type not found", name);
                else
                    Debug.LogFormat("Cache fault for \"{0}\", found type at \"{1}\"", name, result.Assembly.Location);

            cachedTypes[name] = result;
            return result;
        }

        private static Type FindTypeInAssembly(string name, Assembly assembly) {
            return assembly == null ?
                null :
                assembly.GetType(name, false, true);
        }

        public static FieldInfo FindField(this Type type, string fieldName, BindingFlags flags = FULL_BINDING) {
            return type.GetField(fieldName, flags);
        }

        public static PropertyInfo FindProperty(this Type type, string propertyName, BindingFlags flags = FULL_BINDING) {
            return type.GetProperty(propertyName, flags);
        }

        public static MethodInfo FindMethod(this Type type, string methodName, Type[] argsTypes = null, BindingFlags flags = FULL_BINDING) {
            return argsTypes == null ?
                type.GetMethod(methodName, flags) :
                type.GetMethod(methodName, flags, null, argsTypes, null);
        }

        #region Fields
        public static T GetStaticField<T>(this Type type, string fieldName) {
            if (type == null)
                throw new ArgumentNullException("type");
            return (T)type.FindField(fieldName, STATIC_BINDING).GetValue(null);
        }

        public static T GetInstanceField<T>(this object obj, string fieldName) {
            if (obj == null)
                throw new ArgumentNullException("obj");
            return (T)obj.GetType().FindField(fieldName, INSTANCE_BINDING).GetValue(obj);
        }

        public static void SetStaticField<TValue>(this Type type, string fieldName, TValue value) {
            if (type == null)
                throw new ArgumentNullException("type");
            type.FindField(fieldName, STATIC_BINDING).SetValue(null, value);
        }

        public static void SetInstanceField<TObj, TValue>(this TObj obj, string fieldName, TValue value) {
            if (obj == null)
                throw new ArgumentNullException("obj");
            obj.GetType().FindField(fieldName, INSTANCE_BINDING).SetValue(obj, value);
        }

        public static bool HasField(this Type type, string fieldName) {
            return type.FindField(fieldName) != null;
        }

        public static bool HasField<T>(this T obj, string fieldName) {
            return obj.GetType().HasField(fieldName);
        }
        #endregion

        #region Props
        public static T GetStaticProperty<T>(this Type type, string propertyName) {
            if (type == null)
                throw new ArgumentNullException("type");
            return (T)type.FindProperty(propertyName, STATIC_BINDING).GetValue(null, null);
        }

        public static T GetInstanceProperty<T>(this object obj, string propertyName) {
            if (obj == null)
                throw new ArgumentNullException("obj");
            return (T)obj.GetType().FindProperty(propertyName, INSTANCE_BINDING).GetValue(obj, null);
        }

        public static void SetStaticProperty<TValue>(this Type type, string propertyName, TValue value) {
            if (type == null)
                throw new ArgumentNullException("type");
            type.FindProperty(propertyName, STATIC_BINDING).SetValue(null, value, null);
        }

        public static void SetInstanceProperty<TObj, TValue>(this TObj obj, string propertyName, TValue value) {
            if (obj == null)
                throw new ArgumentNullException("obj");
            obj.GetType().FindProperty(propertyName, INSTANCE_BINDING).SetValue(obj, value, null);
        }

        public static bool HasProperty(this Type type, string propertyName) {
            return type.FindProperty(propertyName) != null;
        }

        public static bool HasProperty<T>(this T obj, string propertyName) {
            return obj.GetType().HasProperty(propertyName);
        }
        #endregion

        public static object RawCall(Type type, object obj, string methodName, object[] args, Type[] argsTypes, bool isStatic) {

            if (obj == null && !isStatic)
                throw new ArgumentNullException("obj", "obj cannot be null for instance methods");
            if (type == null)
                throw new ArgumentNullException("type");

            for (var i = 0; i < argsTypes.Length; i++)
                if (argsTypes[i] == typeof(object))
                    argsTypes[i] = args[i].GetType();

            var method = type.FindMethod(methodName, argsTypes, isStatic ? STATIC_BINDING : INSTANCE_BINDING);

            if (method == null)
                throw new MissingMethodException(type.FullName, methodName);

            return method.Invoke(obj, args);
        }

        #region Invoke Instance
        public static void InvokeMethod(this object obj, string methodName) {
            var args = new object[] { };
            var argsTypes = new Type[] { };
            RawCall(obj.GetType(), obj, methodName, args, argsTypes, false);
        }

        public static void InvokeMethod<TArg1>(this object obj, string methodName, TArg1 arg1) {
            var args = new object[] { arg1 };
            var argsTypes = new Type[] { typeof(TArg1) };
            RawCall(obj.GetType(), obj, methodName, args, argsTypes, false);
        }

        public static void InvokeMethod<TArg1, TArg2>(this object obj, string methodName, TArg1 arg1, TArg2 arg2) {
            var args = new object[] { arg1, arg2 };
            var argsTypes = new Type[] { typeof(TArg1), typeof(TArg2) };
            RawCall(obj.GetType(), obj, methodName, args, argsTypes, false);
        }

        public static void InvokeMethod<TArg1, TArg2, TArg3>(this object obj, string methodName, TArg1 arg1, TArg2 arg2, TArg3 arg3) {
            var args = new object[] { arg1, arg2, arg3 };
            var argsTypes = new Type[] { typeof(TArg1), typeof(TArg2), typeof(TArg3) };
            RawCall(obj.GetType(), obj, methodName, args, argsTypes, false);
        }

        public static void InvokeMethod<TArg1, TArg2, TArg3, TArg4>(this object obj, string methodName, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4) {
            var args = new object[] { arg1, arg2, arg3, arg4 };
            var argsTypes = new Type[] { typeof(TArg1), typeof(TArg2), typeof(TArg3), typeof(TArg4) };
            RawCall(obj.GetType(), obj, methodName, args, argsTypes, false);
        }

        public static TResult InvokeMethod<TResult>(this object obj, string methodName) {
            var args = new object[] { };
            var argsTypes = new Type[] { };
            return (TResult)RawCall(obj.GetType(), obj, methodName, args, argsTypes, false);
        }

        public static TResult InvokeMethod<TResult, TArg1>(this object obj, string methodName, TArg1 arg1) {
            var args = new object[] { arg1 };
            var argsTypes = new Type[] { typeof(TArg1) };
            return (TResult)RawCall(obj.GetType(), obj, methodName, args, argsTypes, false);
        }

        public static TResult InvokeMethod<TResult, TArg1, TArg2>(this object obj, string methodName, TArg1 arg1, TArg2 arg2) {
            var args = new object[] { arg1, arg2 };
            var argsTypes = new Type[] { typeof(TArg1), typeof(TArg2) };
            return (TResult)RawCall(obj.GetType(), obj, methodName, args, argsTypes, false);
        }

        public static TResult InvokeMethod<TResult, TArg1, TArg2, TArg3>(this object obj, string methodName, TArg1 arg1, TArg2 arg2, TArg3 arg3) {
            var args = new object[] { arg1, arg2, arg3 };
            var argsTypes = new Type[] { typeof(TArg1), typeof(TArg2), typeof(TArg3) };
            return (TResult)RawCall(obj.GetType(), obj, methodName, args, argsTypes, false);
        }

        public static TResult InvokeMethod<TResult, TArg1, TArg2, TArg3, TArg4>(this object obj, string methodName, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4) {
            var args = new object[] { arg1, arg2, arg3, arg4 };
            var argsTypes = new Type[] { typeof(TArg1), typeof(TArg2), typeof(TArg3), typeof(TArg4) };
            return (TResult)RawCall(obj.GetType(), obj, methodName, args, argsTypes, false);
        }
        #endregion

        #region Invoke Static
        public static void InvokeStaticMethod(this Type type, string methodName) {
            var args = new object[] { };
            var argsTypes = new Type[] { };
            RawCall(type, null, methodName, args, argsTypes, true);
        }

        public static void InvokeStaticMethod<TArg1>(this Type type, string methodName, TArg1 arg1) {
            var args = new object[] { arg1 };
            var argsTypes = new Type[] { typeof(TArg1) };
            RawCall(type, null, methodName, args, argsTypes, true);
        }

        public static void InvokeStaticMethod<TArg1, TArg2>(this Type type, string methodName, TArg1 arg1, TArg2 arg2) {
            var args = new object[] { arg1, arg2 };
            var argsTypes = new Type[] { typeof(TArg1), typeof(TArg2) };
            RawCall(type, null, methodName, args, argsTypes, true);
        }

        public static void InvokeStaticMethod<TArg1, TArg2, TArg3>(this Type type, string methodName, TArg1 arg1, TArg2 arg2, TArg3 arg3) {
            var args = new object[] { arg1, arg2, arg3 };
            var argsTypes = new Type[] { typeof(TArg1), typeof(TArg2), typeof(TArg3) };
            RawCall(type, null, methodName, args, argsTypes, true);
        }

        public static void InvokeStaticMethod<TArg1, TArg2, TArg3, TArg4>(this Type type, string methodName, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4) {
            var args = new object[] { arg1, arg2, arg3, arg4 };
            var argsTypes = new Type[] { typeof(TArg1), typeof(TArg2), typeof(TArg3), typeof(TArg4) };
            RawCall(type, null, methodName, args, argsTypes, true);
        }

        public static TResult InvokeStaticMethod<TResult>(this Type type, string methodName) {
            var args = new object[] { };
            var argsTypes = new Type[] { };
            return (TResult)RawCall(type, null, methodName, args, argsTypes, true);
        }

        public static TResult InvokeStaticMethod<TResult, TArg1>(this Type type, string methodName, TArg1 arg1) {
            var args = new object[] { arg1 };
            var argsTypes = new Type[] { typeof(TArg1) };
            return (TResult)RawCall(type, null, methodName, args, argsTypes, true);
        }

        public static TResult InvokeStaticMethod<TResult, TArg1, TArg2>(this Type type, string methodName, TArg1 arg1, TArg2 arg2) {
            var args = new object[] { arg1, arg2 };
            var argsTypes = new Type[] { typeof(TArg1), typeof(TArg2) };
            return (TResult)RawCall(type, null, methodName, args, argsTypes, true);
        }

        public static TResult InvokeStaticMethod<TResult, TArg1, TArg2, TArg3>(this Type type, string methodName, TArg1 arg1, TArg2 arg2, TArg3 arg3) {
            var args = new object[] { arg1, arg2, arg3 };
            var argsTypes = new Type[] { typeof(TArg1), typeof(TArg2), typeof(TArg3) };
            return (TResult)RawCall(type, null, methodName, args, argsTypes, true);
        }

        public static TResult InvokeStaticMethod<TResult, TArg1, TArg2, TArg3, TArg4>(this Type type, string methodName, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4) {
            var args = new object[] { arg1, arg2, arg3, arg4 };
            var argsTypes = new Type[] { typeof(TArg1), typeof(TArg2), typeof(TArg3), typeof(TArg4) };
            return (TResult)RawCall(type, null, methodName, args, argsTypes, true);
        }
        #endregion

        #region Has Method
        public static bool HasMethod(this object obj, string methodName) {
            var argsTypes = new Type[] { };
            return obj.GetType().FindMethod(methodName, argsTypes) != null;
        }

        public static bool HasMethod<TArg1>(this object obj, string methodName) {
            var argsTypes = new Type[] { typeof(TArg1) };
            return obj.GetType().FindMethod(methodName, argsTypes) != null;
        }

        public static bool HasMethod<TArg1, TArg2>(this object obj, string methodName) {
            var argsTypes = new Type[] { typeof(TArg1), typeof(TArg2) };
            return obj.GetType().FindMethod(methodName, argsTypes) != null;
        }

        public static bool HasMethod<TArg1, TArg2, TArg3>(this object obj, string methodName) {
            var argsTypes = new Type[] { typeof(TArg1), typeof(TArg2), typeof(TArg3) };
            return obj.GetType().FindMethod(methodName, argsTypes) != null;
        }

        public static bool HasMethod<TArg1, TArg2, TArg3, TArg4>(this object obj, string methodName) {
            var argsTypes = new Type[] { typeof(TArg1), typeof(TArg2), typeof(TArg3), typeof(TArg4) };
            return obj.GetType().FindMethod(methodName, argsTypes) != null;
        }

        public static bool HasMethod(this Type type, string methodName) {
            var argsTypes = new Type[] { };
            return type.FindMethod(methodName, argsTypes) != null;
        }

        public static bool HasMethod<TArg1>(this Type type, string methodName) {
            var argsTypes = new Type[] { typeof(TArg1) };
            return type.FindMethod(methodName, argsTypes) != null;
        }

        public static bool HasMethod<TArg1, TArg2>(this Type type, string methodName) {
            var argsTypes = new Type[] { typeof(TArg1), typeof(TArg2) };
            return type.FindMethod(methodName, argsTypes) != null;
        }

        public static bool HasMethod<TArg1, TArg2, TArg3>(this Type type, string methodName) {
            var argsTypes = new Type[] { typeof(TArg1), typeof(TArg2), typeof(TArg3) };
            return type.FindMethod(methodName, argsTypes) != null;
        }

        public static bool HasMethod<TArg1, TArg2, TArg3, TArg4>(this Type type, string methodName) {
            var argsTypes = new Type[] { typeof(TArg1), typeof(TArg2), typeof(TArg3), typeof(TArg4) };
            return type.FindMethod(methodName, argsTypes) != null;
        }
        #endregion

    }
}
