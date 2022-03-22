using System;
using System.Reflection;
using Terraria.ModLoader;

namespace AQMod.Common.HookLists
{
    internal class LoadHookAttribute : Attribute
    {
        private readonly string modName;
        private readonly string modType;
        private readonly string methodName;
        private readonly BindingFlags flags;

        public MethodInfo BaseMethodForON;

        public LoadHookAttribute(Type type, string methodName) : this(type, methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
        {
        }

        public LoadHookAttribute(Type type, string methodName, BindingFlags flags)
        {
            BaseMethodForON = type.GetMethod(methodName, flags);
        }

        public LoadHookAttribute(MethodInfo methodInfo)
        {
            BaseMethodForON = methodInfo;
        }

        public LoadHookAttribute(string modName, string modType, string method) : this(modName, modType, method, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
        {
        }

        public LoadHookAttribute(string modName, string modType, string method, BindingFlags flags)
        {
            this.modName = modName;
            this.modType = modType;
            methodName = method;
            this.flags = flags;
        }

        public void LoadtimeChecks()
        {
            if (!string.IsNullOrEmpty(modName))
            {
                var m = ModLoader.GetMod(modName);
                if (m != null)
                {
                    var t = m.Code.GetType(modType);
                    if (t == null)
                    {
                        AQMod.Instance.Logger.Error("Type {" + modType + "} was not found");
                        return;
                    }
                    BaseMethodForON = t.GetMethod(methodName, flags);
                    if (BaseMethodForON == null)
                    {
                        AQMod.Instance.Logger.Error("Method {" + methodName + "} from {" + modType + "} was not found");
                        return;
                    }
                }
            }
        }
    }
}