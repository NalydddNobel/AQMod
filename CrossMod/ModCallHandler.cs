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
            dict[m.Name] = m;
        }

        return dict;
    }

    public object? Call(object[] arguments) {
        if (arguments[0] is not string name) {
            return Failure;
        }

        try {
            return _calls[name].Invoke(null, arguments);
        }
        catch (System.Exception ex) {
            Mod!.Logger.Error(ex);
            return Failure;
        }
    }
}
