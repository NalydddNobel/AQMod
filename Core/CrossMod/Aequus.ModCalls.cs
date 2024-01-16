using Aequus.Core.CrossMod;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

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
    public static bool ExpertDropsInClassicMode(Assembly caller, [Optional] bool? value) {
        if (value.HasValue) {
            ModCommons.ExpertDropsInClassicMode = value.Value;
            Instance.Logger.Info($"Expert Drops in Classic Mode set to {ModCommons.ExpertDropsInClassicMode} by {caller.Name()}.");
        }

        return ModCommons.ExpertDropsInClassicMode;
    }

    #region Backend
    private class CallMethod {
        public readonly string MethodName;
        private readonly MethodInfo _innerMethod;
        private readonly ParameterInfo[] _parameters;

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

    private readonly Dictionary<string, CallMethod> Calls = new();

    private void LoadModCalls() {
        foreach (var call in typeof(Aequus).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)) {
            var modCallAttribute = call.GetCustomAttribute<ModCallAttribute>();
            if (modCallAttribute == null) {
                continue;
            }

            var callMethod = new CallMethod(call);
            Calls.Add(callMethod.MethodName, callMethod);
            foreach (var alias in modCallAttribute._aliases) {
                Calls.Add(alias, callMethod);
            }
        }

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