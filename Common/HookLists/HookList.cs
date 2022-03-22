using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using System.Reflection;

namespace AQMod.Common.HookLists
{
    public abstract class HookList : IAutoloadType
    {
        public void Load()
        {
            PreLoadHooks();
            LoadHooks();
            PostLoadHooks();
        }

        public void Unload()
        {
            OnUnload();
        }

        private void LoadHooks()
        {
            var methods = GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            foreach (var method in methods)
            {
                var attr = method.GetCustomAttribute<LoadHookAttribute>();
                if (attr != null)
                {
                    var args = method.GetParameters();
                    if (args.Length == 1 && args[0].GetType() == typeof(ILContext))
                    {

                    }
                    else
                    {
                        attr.LoadtimeChecks();
                        if (attr.BaseMethodForON != null)
                        {
                            AddHook(attr.BaseMethodForON, method);
                        }
                    }
                }
            }
        }

        protected virtual void PreLoadHooks()
        {

        }

        protected virtual void PostLoadHooks()
        {

        }

        protected virtual void OnUnload()
        {

        }

        public static bool AddHook(MethodInfo baseMethod, MethodInfo newMethod)
        {
            if (baseMethod == null)
            {
                AQMod.Instance.Logger.Error("The base method was null");
                return false;
            }
            if (newMethod == null)
            {
                AQMod.Instance.Logger.Error("The new method was null");
                return false;
            }
            new Hook(baseMethod, newMethod).Apply();
            //AQMod.Instance.Logger.Error("Hook {" + newMethod.Name + "} was applied successfully onto {" + baseMethod.Name + "}");
            return true;
        }
    }
}
