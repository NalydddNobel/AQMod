using System.Reflection;

namespace Aequus.Common.Utilities
{
    public static class ReflectionHelper
    {
        public static T GetValue<T>(this PropertyInfo property, object obj)
        {
            return (T)property.GetValue(obj);
        }
        public static T GetValue<T>(this FieldInfo field, object obj)
        {
            return (T)field.GetValue(obj);
        }
        public static T ReflectiveCloneTo<T>(this T obj, T obj2)
        {
            return ReflectiveCloneTo(obj, obj2, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        }
        public static T ReflectiveCloneTo<T>(this T obj, T obj2, BindingFlags flags)
        {
            var t = typeof(T);
            foreach (var f in t.GetFields(flags))
            {
                if (!f.IsInitOnly)
                {
                    f.SetValue(obj2, f.GetValue(obj));
                }
            }
            foreach (var p in t.GetProperties(flags))
            {
                if (p.CanWrite)
                {
                    p.SetValue(obj2, p.GetValue(obj));
                }
            }
            return obj2;
        }
    }
}