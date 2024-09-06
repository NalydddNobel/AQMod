using Aequus.Common;
using System.Collections.Generic;
using System.Reflection;

namespace Aequus;

public partial class ModCallHandler : LoadedType {
    private const string Failure = "Failure";
    private readonly Dictionary<string, MethodInfo> _calls = CollectCalls();

    public static ModCallHandler Instance => ModContent.GetInstance<ModCallHandler>();

    private static Dictionary<string, MethodInfo> CollectCalls() {
        Dictionary<string, MethodInfo> dict = [];

        foreach (MethodInfo m in typeof(ModCallHandler).GetMethods(BindingFlags.Static | BindingFlags.NonPublic)) {
            dict[m.Name.ToLower()] = m;
        }

        return dict;
    }

    private static void LogError(string message) {
        Instance.Mod!.Logger.Error(message);
    }

    private static void LogInfo(string message) {
        Instance.Mod!.Logger.Info(message);
    }

    public object? Call(object[] arguments) {
        if (arguments[0] is not string name) {
            return Failure;
        }

        try {
            MethodInfo method = _calls[name.ToLower()];
            object? value = method.Invoke(null, [arguments]);
            return value;
        }
        catch (System.Exception ex) {
            LogError(ex.ToString());
            return Failure;
        }
    }
}
