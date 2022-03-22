using MonoMod.Cil;
using System.Collections.Generic;
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
                            AQUtils.AddHook(attr.BaseMethodForON, method);
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
    }
}
