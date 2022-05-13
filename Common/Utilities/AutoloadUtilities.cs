using System;
using System.Reflection;
using Terraria.ModLoader;

namespace Aequus.Common.Utilities
{
    internal static class AutoloadUtilities
    {
        public static Type[] GetTypesFor(Assembly ass)
        {
            return ass.GetTypes();
        }

        public static bool HasInstanceOf<T>(Type t, out T instance) where T : class
        {
            instance = null;
            foreach (var i in t.GetInterfaces())
            {
                if (i.IsAssignableTo(typeof(T)))
                {
                    instance = ModContent.GetInstance<T>();
                    return instance != null;
                }
            }

            return false;
        }
    }
}