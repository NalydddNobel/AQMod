using System;
using System.Reflection;

namespace AQMod.Common.ID
{
    [AttributeUsage(AttributeTargets.Class)]
    internal sealed class SetConstantsIdentityAttribute : Attribute
    {
        public readonly int Count;

        public SetConstantsIdentityAttribute(Type type)
        {
            Count = (int)type.GetField("Count", BindingFlags.Public | BindingFlags.Static).GetValue(null);
        }

        public static int GetCount<T>() where T : class
        {
            return typeof(T).GetCustomAttribute<SetConstantsIdentityAttribute>().Count;
        }
    }
}