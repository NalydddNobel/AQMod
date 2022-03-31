using System.Reflection;

namespace Aequus.Common.Utilities
{
    public static class MiscHelper
    {
        public static T GetValue<T>(this PropertyInfo property, object obj)
        {
            return (T)property.GetValue(obj);
        }
        public static T GetValue<T>(this FieldInfo field, object obj)
        {
            return (T)field.GetValue(obj);
        }
    }
}