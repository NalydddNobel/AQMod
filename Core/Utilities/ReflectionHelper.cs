using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AequusRemake.Core.Utilities;

public static class ReflectionHelper {
    public static bool HasAttribute<T>(this object instance) where T : Attribute {
        return instance?.GetType()?.GetCustomAttribute<T>() != null;
    }
    public static bool HasAttribute<TType, TAttribute>() where TAttribute : Attribute {
        return typeof(TType).GetCustomAttribute<TAttribute>() != null;
    }

    /// <returns>Returns all constant fields from <paramref name="obj"/>'s <see cref="Type"/>.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static IEnumerable<FieldInfo> GetConstantFields(this object obj, BindingFlags flags = BindingFlags.Public) {
        if (obj == null) {
            throw new ArgumentNullException(nameof(obj));
        }
        return GetConstantFields(obj.GetType(), flags);
    }
    /// <returns>Returns all constant fields from <paramref name="type"/>.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static IEnumerable<FieldInfo> GetConstantFields(this Type type, BindingFlags flags = BindingFlags.Public) {
        if (type == null) {
            throw new ArgumentNullException(nameof(type));
        }

        flags |= BindingFlags.Static;
        foreach (FieldInfo fieldInfo in type.GetFields().Where(f => f.IsLiteral && !f.IsInitOnly)) {
            yield return fieldInfo;
        }
    }

    public static IEnumerable<(T attributeInstance, MemberInfo memberInfo)> GetMembersWithAttribute<T>(Type t, BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static) where T : Attribute {
        var l = new List<(T, MemberInfo)>();
        foreach (var f in t.GetFields(flags)) {
            var attr = f.GetCustomAttribute<T>();
            if (attr != null) {
                l.Add((attr, f));
            }
        }
        foreach (var p in t.GetProperties(flags)) {
            var attr = p.GetCustomAttribute<T>();
            if (attr != null) {
                l.Add((attr, p));
            }
        }
        return l;
    }

    public static T GetValue<T>(this PropertyInfo property, object obj) {
        return (T)property.GetValue(obj);
    }
    public static T GetValue<T>(this FieldInfo field, object obj) {
        return (T)field.GetValue(obj);
    }

    public static object GetDefaultValue(Type t) {
        return t.IsValueType ? Activator.CreateInstance(t) : null;
    }

    public static bool HasMethodOverride(this Type t, string methodName, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public) {
        foreach (MethodInfo method in t.GetMethods(bindingFlags).Where(x => x.Name == methodName)) {
            if (method.GetBaseDefinition().DeclaringType != method.DeclaringType) {
                return true;
            }
        }

        return false;
    }
}