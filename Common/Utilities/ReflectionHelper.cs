using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Aequus.Common.Utilities;

public static class ReflectionHelper {
    public static IEnumerable<(T attributeInstance, MemberInfo memberInfo)> GetMembersWithAttribute<T>(Type t) where T : Attribute {
        var l = new List<(T, MemberInfo)>();
        foreach (var f in t.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)) {
            var attr = f.GetCustomAttribute<T>();
            if (attr != null) {
                l.Add((attr, f));
            }
        }
        foreach (var p in t.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)) {
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