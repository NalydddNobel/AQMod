using System.Reflection;

namespace Aequus.Core.Utilities;
public static class ReflectionHelper {
    public static T GetValue<T>(this PropertyInfo property, object obj) {
        return (T)property.GetValue(obj);
    }
    public static T GetValue<T>(this FieldInfo field, object obj) {
        return (T)field.GetValue(obj);
    }
}