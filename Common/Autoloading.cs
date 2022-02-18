using AQMod.Common.Utilities.Debugging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria;

namespace AQMod.Common
{
    internal static class Autoloading
    {
        private static List<IAutoloadType> _autoloadCache;
        
        private static void TryAutoload(Type t, DebugUtilities.Logger? logger)
        {
            if (!t.IsAbstract && t.GetInterfaces().Contains(typeof(IAutoloadType)))
            {
                var instance = (IAutoloadType)Activator.CreateInstance(t);
                instance.OnLoad();
                logger?.Log("Created autoload instance of: {0}", t.FullName);
                _autoloadCache.Add(instance);
            }
        }
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
                try
                {
                    TryAutoload(t, logger);
                }
                catch (Exception ex)
                {
                    // throws error if not loading in Multiplayer
                    if (!Main.dedServ)
                    {
                        throw ex;
                    }
                    else
                    {
                        AQMod.GetInstance().Logger.Error(ex);
                    }
                }
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