using log4net;
using System;
using Terraria.ModLoader;

namespace AQMod.Common.DeveloperTools
{
    internal sealed class aqdebug
    {
        private static bool oldLogAccess;

        public static bool LogAccess = true;

        public static bool LogAutoload = false;
        public static bool LogDyeBinding = false;
        public static bool LogEffectLoading = false;
        public static bool LogNetcode = true;
        public static bool LogTextureLoading = false;
        public static bool LogModCallObjectInitialization = false;
        public static bool LogModCalls = true;

        public struct DebugLogger
        {
            private readonly ILog _logger;

            public DebugLogger(ILog logger)
            {
                _logger = logger;
            }

            public void Log(string message)
            {
                _logger.Debug(message);
            }

            public void Log(string message, object arg0)
            {
                _logger.Debug(string.Format(message.ToString(), arg0));
            }

            public void Log(string message, params object[] args)
            {
                _logger.Debug(string.Format(message.ToString(), args));
            }

            public void Log(string message, Exception exception)
            {
                _logger.Debug(message, exception);
            }

            public void Log(object message)
            {
                _logger.Debug(message);
            }

            public void Log(object message, object arg0)
            {
                _logger.Debug(string.Format(message.ToString(), arg0));
            }

            public void Log(object message, params object[] args)
            {
                _logger.Debug(string.Format(message.ToString(), args));
            }

            public void Log(object message, Exception exception)
            {
                _logger.Debug(message, exception);
            }
        }

        public static DebugLogger GetDebugLogger()
        {
            var logger = AQMod.GetInstance().Logger;
            InternalLogAccess(logger);
            return new DebugLogger(logger);
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