using AequusRemake.Core.Hooks;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using System;
using System.Reflection;

namespace AequusRemake.Core.Util.Helpers;
public sealed class ILTool {
    [Obsolete]
    public static void DeleteMethod(MethodInfo Method, object returnValue) {
        ILHook hook = new ILHook(Method, Delete);
        HookManager._loadedHooks.Add(hook);
        hook.Apply();

        static void Delete(ILContext il) {
            il.Instrs.Clear();
            //il.Instrs.Add(il.IL.Create(OpCodes.ld, returnValue));
            il.Instrs.Add(il.IL.Create(OpCodes.Ret));
        }
    }

    public static void DeleteMethod(MethodInfo Method) {
        ILHook hook = new ILHook(Method, Delete);
        HookManager._loadedHooks.Add(hook);
        hook.Apply();

        static void Delete(ILContext il) {
            il.Instrs.Clear();
            il.Instrs.Add(il.IL.Create(OpCodes.Ret));
        }
    }

    public static void DeleteMethod(Type type, string MethodName) {
        MethodInfo method = type.GetMethod(MethodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
        if (method != null) {
            DeleteMethod(method);
        }
        else {
            Log.Error($"Method \"{MethodName}\" not found in {type.FullName}.");
        }
    }
}
