using AQMod.Common.Utilities.Debugging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AQMod.Common
{
    internal static class Autoloading
    {
        private static List<IAutoloadType> _autoloadCache;

        public static void Autoload(Assembly code)
        {
            _autoloadCache = new List<IAutoloadType>();
            DebugUtilities.Logger? logger = null;
            if (DebugUtilities.LogAutoload)
            {
                logger = DebugUtilities.GetDebugLogger();
            }

            foreach (var t in code.GetTypes())
            {
                InternalAutoload(t, logger);
            }
        }
        private static void InternalAutoload(Type t, DebugUtilities.Logger? logger)
        {
            if (!t.IsAbstract && t.GetInterfaces().Contains(typeof(IAutoloadType)))
            {
                var instance = (IAutoloadType)Activator.CreateInstance(t);
                instance.Load();
                logger?.Log("Created autoload instance of: {0}", t.FullName);
                _autoloadCache.Add(instance);
            }
        }

        public static void SetupContent(Assembly code)
        {
            if (DebugUtilities.LogAutoload)
            {
                var logger = DebugUtilities.GetDebugLogger();
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