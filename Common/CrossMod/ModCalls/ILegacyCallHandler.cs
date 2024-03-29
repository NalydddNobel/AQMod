﻿namespace Aequus.Common.CrossMod.ModCalls {
    public interface ILegacyCallHandler {
        ILegacyCallHandler HandleArg(string name, object value);

        public static ILegacyCallHandler HandleArgs(ILegacyCallHandler obj, int start, object[] args) {
            if (args.Length < start + 2) {
                return obj;
            }
            while (true) {
                if (args.Length < start + 1) {
                    Aequus.Instance.Logger.Error("Invalid arg lengths. Safely leaving handler, final arg: " + (args[^1] == null ? "NULL" : args[^1].ToString()));
                    return obj;
                }

                if (args[start] is string name) {
                    obj = obj.HandleArg(name, args[start + 1]);
                }
                else {
                    Aequus.Instance.Logger.Error("Invalid arg type '" + args[start].GetType().FullName + "', string expected. Safely leaving handler, arg value: " + (args[start] == null ? "NULL" : args[start].ToString()));
                    return obj;
                }
                start += 2;

                if (args.Length < start + 2) {
                    return obj;
                }
            }
        }

        protected static void DoesntExistReport(string name, ILegacyCallHandler me) {
            Aequus.Instance.Logger.Error(name + " doesn't exist in " + me.GetType().Name);
        }
        protected static void SuccessReport(string name, object value, ILegacyCallHandler me) {
            if (Aequus.InfoLogs)
                Aequus.Instance.Logger.Info("Setting " + name + " in " + me.GetType().Name + " to " + (value == null ? "NULL" : value.ToString()));
        }
    }
}