using System;
using System.Reflection;

namespace AQMod.Common.DeveloperTools
{
    [AttributeUsage(AttributeTargets.Class)]
    internal sealed class IdentityAttribute : Attribute
    {
        public readonly int Count;

        public IdentityAttribute(Type type)
        {
            Count = (int)type.GetField("Count", BindingFlags.Public | BindingFlags.Static).GetValue(null);
        }

        public static int GetCount<T>() where T : class
        {
            return typeof(T).GetCustomAttribute<IdentityAttribute>().Count;
        }
    }
}