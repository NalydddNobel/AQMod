using AQMod.Common.DeveloperTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AQMod.Common
{
    internal static class Autoloading
    {
        public static bool LoadingArmorSets;

        private static List<IAutoloadType> _autoloadCache;

        public static void Autoload(Assembly code)
        {
            _autoloadCache = new List<IAutoloadType>();
            if (aqdebug.LogAutoload)
            {
                var logger = aqdebug.GetDebugLogger();
                foreach (var t in code.GetTypes())
                {
                    if (!t.IsAbstract && t.GetInterfaces().Contains(typeof(IAutoloadType)))
                    {
                        var instance = (IAutoloadType)Activator.CreateInstance(t);
                        instance.OnLoad();
                        logger.Log("Created autoload instance of: {0}", t.FullName);
                        _autoloadCache.Add(instance);
                    }
                }
            }
            else
            {
                foreach (var t in code.GetTypes())
                {
                    if (!t.IsAbstract && t.GetInterfaces().Contains(typeof(IAutoloadType)))
                    {
                        var instance = (IAutoloadType)Activator.CreateInstance(t);
                        instance.OnLoad();
                        _autoloadCache.Add(instance);
                    }
                }
            }
        }

        public static void SetupContent(Assembly code)
        {
            if (aqdebug.LogAutoload)
            {
                var logger = aqdebug.GetDebugLogger();
                foreach (var t in code.GetTypes())
                {
                    if (!t.IsAbstract && t.GetInterfaces().Contains(typeof(ISetupContentType)))
                    {
                        var instance = (ISetupContentType)Activator.CreateInstance(t);
                        logger.Log("Created autoload instance of: {0}", t.FullName);
                        instance.SetupContent();
                    }
                }
            }
            else
            {
                foreach (var t in code.GetTypes())
                {
                    if (!t.IsAbstract && t.GetInterfaces().Contains(typeof(ISetupContentType)))
                    {
                        var instance = (ISetupContentType)Activator.CreateInstance(t);
                        instance.SetupContent();
                    }
                }
            }
        }

        public static void Unload()
        {
            if (_autoloadCache == null)
                return;
            foreach (var autoload in _autoloadCache)
            {
                autoload.Unload();
            }
        }
    }
}