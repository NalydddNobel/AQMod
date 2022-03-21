using log4net;
using System;
using Terraria.ModLoader;

namespace AQMod.Common.Utilities.Debugging
{
    internal sealed class DebugUtilities
    {
        private static bool oldLogAccess;

        public static bool LogAccess = false;

        public static bool LogAutoload = false;
        public static bool LogDyeBinding = false;
        public static bool LogEffectLoading = false;
        public static bool LogNetcode = true;
        public static bool LogTextureLoading = false;
        public static bool LogModCallObjectInitialization = false;
        public static bool LogModCalls = true;

        public struct Logger
        {
            private readonly ILog internalLogger;

            public Logger(ILog logger)
            {
                internalLogger = logger;
            }

            public void Log(string message)
            {
                internalLogger.Debug(message);
            }

            public void Log(string message, object arg0)
            {
                internalLogger.Debug(string.Format(message.ToString(), arg0));
            }

            public void Log(string message, params object[] args)
            {
                internalLogger.Debug(string.Format(message.ToString(), args));
            }

            public void Log(string message, Exception exception)
            {
                internalLogger.Debug(message, exception);
            }

            public void Log(object message)
            {
                internalLogger.Debug(message);
            }

            public void Log(object message, object arg0)
            {
                internalLogger.Debug(string.Format(message.ToString(), arg0));
            }

            public void Log(object message, params object[] args)
            {
                internalLogger.Debug(string.Format(message.ToString(), args));
            }

            public void Log(object message, Exception exception)
            {
                internalLogger.Debug(message, exception);
            }
        }

        public static Logger GetDebugLogger()
        {
            var logger = AQMod.Instance.Logger;
            InternalLogAccess(logger);
            return new Logger(logger);
        }

        public static Logger? GetDebugLogger(bool get)
        {
            if (!get)
            {
                return null;
            }
            var logger = AQMod.Instance.Logger;
            InternalLogAccess(logger);
            return new Logger(logger);
        }

        private static void InternalLogAccess(ILog logger)
        {
            if (LogAccess)
                logger.Info("Accessed debug logger at: " + Environment.StackTrace);
        }

        public static void SupressLogAccessMessage()
        {
            oldLogAccess = LogAccess;
            InternalLogAccess(ModContent.GetInstance<AQMod>().Logger);
            LogAccess = false;
        }

        public static void RepairLogAccessMessage()
        {
            LogAccess = oldLogAccess;
        }
    }
}