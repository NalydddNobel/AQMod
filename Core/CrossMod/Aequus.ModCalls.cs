using Aequus.Core.CrossMod;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using Terraria.Social.WeGame;

namespace Aequus;

// Check https://terrariamods.wiki.gg/wiki/Aequus/Mod_Calls for more info.

public partial class Aequus {
    #region Fargos
    [ModCall]
    public static bool AbominationnClearEvents(bool canClearEvents) {
        return false;
    }
    #endregion

    [ModCall]
    public static bool RemoveExpertExclusivity(Assembly caller, [Optional] bool? value) {
        if (value.HasValue) {
            ModCompatabilityFlags.RemoveExpertExclusivity = value.Value;
            Instance.Logger.Info($"Remove Expert Exclusivity set to {ModCompatabilityFlags.RemoveExpertExclusivity} by {caller.Name()}.");
        }

        return ModCompatabilityFlags.RemoveExpertExclusivity;
    }

    #region Backend
    private class CallMethod {
        public readonly string MethodName;
        internal readonly MethodInfo _innerMethod;
        internal readonly ParameterInfo[] _parameters;

        public CallMethod(MethodInfo method) {
            MethodName = method.Name;
            _innerMethod = method;
            _parameters = method.GetParameters();
        }

        // Note: Exceptions are RETURNED to the caller
        // This lets them handle or ignore the exceptions themselves without needing a try-catch case.
        public object TryInvoke(Assembly caller, object[] args) {
            int argumentIndex = 1;
            // TODO -- Make this not initialize a new array for each call?
            object[] methodParameters = new object[_parameters.Length];
            for (int i = 0; i < _parameters.Length; i++) {
                var wantedType = _parameters[i].ParameterType;
                // Set method's "Assembly" parameter to the caller.
                if (wantedType == typeof(Assembly)) {
                    methodParameters[i] = caller;
                    continue;
                }

                bool outOfBounds = argumentIndex >= args.Length;
                // Check if parameter is Optional
                if (_parameters[i].GetCustomAttribute<OptionalAttribute>() != null && (outOfBounds || args[argumentIndex] == null)) {
                    // Set it to default value
                    if (wantedType.IsValueType) {
                        methodParameters[i] = Activator.CreateInstance(wantedType);
                    }
                    else {
                        methodParameters[i] = null;
                    }
                    // We do not care if we're out of bounds, since we don't need any argument info from the caller
                    continue;
                }

                if (outOfBounds) {
                    // Return out of bounds exception 
                    return new IndexOutOfRangeException($"Invalid amount of arguments provided for mod call from {caller.Name()}.");
                }

                var argumentType = args[argumentIndex].GetType();
                if (wantedType != argumentType) {
                    // Check if the type mis-match is because of Nullable<T>
                    if (wantedType.GetGenericTypeDefinition() == typeof(Nullable<>)) {
                        // If the nullable type argument doesn't match the argument type
                        if (wantedType.GenericTypeArguments[0] != argumentType) {
                            return new ArgumentException($"Argument {i} must be of Type {wantedType.GenericTypeArguments[0].FullName} or null. Type provided was {argumentType.FullName}.");
                        }

                        // Otherwise, we are free to set the method parameter to this argument,
                        // as it matches the Nullable<T>'s generic type.
                    }
                    else {
                        // Return exception for type mis-match.
                        return new ArgumentException($"Argument {i} must be of Type {wantedType.FullName}. Type provided was {argumentType.FullName}.");
                    }
                }

                methodParameters[i] = args[argumentIndex];

                argumentIndex++;
            }

            return _innerMethod.Invoke(null, methodParameters);
        }
    }

    private readonly Dictionary<string, (CallMethod, string[])> NonAliasCalls = new();
    private readonly Dictionary<string, CallMethod> Calls = new();

    private void LoadModCalls() {
        foreach (var call in typeof(Aequus).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)) {
            var modCallAttribute = call.GetCustomAttribute<ModCallAttribute>();
            if (modCallAttribute == null) {
                continue;
            }

            var callMethod = new CallMethod(call);
            NonAliasCalls.Add(callMethod.MethodName, (callMethod, modCallAttribute._aliases));
            Calls.Add(callMethod.MethodName, callMethod);
            foreach (var alias in modCallAttribute._aliases) {
                Calls.Add(alias, callMethod);
            }
        }

        GenerateWikiPage();
        // Test Mod calls
        //TestCall("");
        //TestCall("Pleh");
        //TestCall("ExpertDropsInClassicMode");
        //TestCall("ExpertDropsInClassicMode", null);
        //TestCall("ExpertDropsInClassicMode", true);
        //TestCall("ExpertDropsInClassicMode", false);
        //TestCall("ExpertDropsInClassicMode", 0);
        //TestCall("ExpertDropsInClassicMode", 0f);
    }

    private void UnloadModCalls() {
        Calls.Clear();
    }

    public override object Call(params object[] args) {
        Assembly caller = Assembly.GetCallingAssembly();

        if (args[0] is not string key) {
            return new ArgumentException($"All mod calls must have a string identifier (argument 0, Check https://terrariamods.wiki.gg/wiki/Aequus/Mod_Calls for more info.). From Mod: {caller.Name()}.");
        }

        if (Calls.TryGetValue(key, out var call)) {
            object result = call.TryInvoke(caller, args);
            return result;
        }

        var notFoundException = new ArgumentException($"Mod Call: \"{key}\" was not found. From Mod: {caller.Name()}.");
        return notFoundException;
    }

    [Conditional("DEBUG")]
    private void TestCall(params object[] args) {
        object result = Call(args);

        if (result is Exception ex) {
            Instance.Logger.Error(ex.Message + "\n" + ex.StackTrace);
        }
    }

    [Conditional("DEBUG")]
    private void GenerateWikiPage() {
        string path = $"{DEBUG_FILES_PATH}/ModSources/Aequus/Assets/Metadata/ModCallsWikiPage.Temp";
        if (!Directory.Exists(Path.GetDirectoryName(path))) {
            return;
        }

        try {
            using StreamWriter w = new StreamWriter(File.Create(path));

            w.Write("{{mod sub-page}}<!--DO NOT REMOVE THIS LINE! It is required for Mod sub-pages to work properly.-->\r\nMod Calls are special functions provided by tModLoader that mod creators can use to interact with Aequus in certain ways. They are accessible through <code>Mod.Call</code> and a given call name; for more details, see the [https://github.com/tModLoader/tModLoader/wiki/Expert-Cross-Mod-Content#call-aka-modcall-intermediate tModLoader wiki]. This page lists the mod call functions Aequus currently adds.\r\n");
            w.Write("\r\nThis page expects you to have an understanding of Terraria mod coding, and how to utilize Mod.Call.\r\n");
            w.Write("\r\nIf you would like to view the code for handling mod calls, you can find it [https://github.com/NalydddNobel/AQMod/blob/reboot/Core/CrossMod/Aequus.ModCalls.cs here]\r\n");
            w.Write("\r\n== Important Information ==\r\nAll Calls are required to have atleast 1 parameter. A <code>string</code>, which is the name of the call.\r\n");
            w.Write("\r\n== Examples ==\r\nThe simplest way to get an instance of Aequus:\r\n if (ModLoader.TryGetMod(\"Aequus\", out Mod aequus))\r\n }\r\n     // A call goes here\r\n }\r\n\r\nExample of some mod calls.\r\n aequus.Call(\"downedCrabson\", Mod); // Returns whether Crabson has been defeated.\r\n aequus.Call(\"downedCrabson\", Mod, true); // Sets Crabson to defeated.\r\n");
            w.Write("\r\n== Mod Calls ==");
            w.Write("\r\n{| class=\"terraria sortable lined align-center\"\r\n! class=unsortable | Call !! Parameters !! Result");
            foreach (var call in NonAliasCalls) {
                w.Write($"\r\n|-");
                w.Write($"\r\n|| <div style=\"float:left;\">'''{call.Key}'''</div>");
                // Should we write call aliases? They are technically supported but not preferred to be used.
                if (call.Value.Item1._parameters.Length != 0) {
                    w.Write($"\r\n|| <div style=\"float:left;\">{string.Join("</div>\r\n<br><div style=\"float:left;\">", call.Value.Item1._parameters.Where(p => p.ParameterType != typeof(Assembly)).Select(p => "• " + GetParameterInfo(p)))}</div>");
                }
                else {
                    w.Write("\r\n|| <div style=\"float:left;\">None</div>");
                }
                w.Write($"\r\n|| {(call.Value.Item1._innerMethod.ReturnType == null ? "null" : call.Value.Item1._innerMethod.ReturnType.Name)}");
            }
            w.Write("\r\n|}");
        }
        catch { }
        finally { }

        static string GetParameterInfo(ParameterInfo p) {
            string typeName = GetTypeName(p.ParameterType);
            if (p.GetCustomAttribute<OptionalAttribute>() != null) {
                typeName = "''(Optional)'' " + typeName;
            }
            return $"{typeName} ({p.Name})";
        }

        static string GetTypeName(Type type) {
            string typeName;
            if (type.IsGenericType) {
                if (type.GetGenericTypeDefinition() == typeof(Nullable<>)) {
                    return GetTypeName(type.GenericTypeArguments[0]);
                }
                else {
                    typeName = type.Name[..type.Name.IndexOf('`')] + "{";
                    // Recursion, yay!
                    typeName += string.Join(", ", type.GenericTypeArguments.Select(g => GetTypeName(g)));
                    typeName += "}";

                    return typeName;
                }
            }

            return type.Name;
        }
    }
    #endregion
}

[AttributeUsage(AttributeTargets.Method)]
internal class ModCallAttribute : Attribute {
    public readonly string[] _aliases;

    public ModCallAttribute(params string[] aliases) {
        _aliases = aliases;
    }
}

internal static class ModCallExtensions {
    public static string Name(this Assembly ass) {
        return ass.GetName().Name ?? "Unknown";
    }
}