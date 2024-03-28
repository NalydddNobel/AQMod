using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Aequus.Core.CodeGeneration;

/// <summary>Rough system which helps getting private fields from classes.</summary>
public class Publicization<T, TReturnValue> {
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="NullReferenceException"></exception>
    /// <exception cref="MissingMemberException"></exception>
    /// <exception cref="NotSupportedException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TReturnValue Get(string name) {
        return Get(default(T), name);
    }
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="NullReferenceException"></exception>
    /// <exception cref="MissingMemberException"></exception>
    /// <exception cref="NotSupportedException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public static TReturnValue Get(T obj, string name) {
        GetDelegate getter = (CollectionsMarshal.GetValueRefOrAddDefault(_delegates, name, out _) ??= GenerateGetter(name));
        TReturnValue value = getter.Invoke(obj);
        //Aequus.Log.Info(value?.ToString() ?? "null");
        return value;
    }

    #region Backend
    public delegate TReturnValue GetDelegate(T obj);

    private static Dictionary<string, GetDelegate> _delegates = new();

    private static GetDelegate GenerateGetter(string memberName) {
        FieldInfo field = typeof(T).GetField(memberName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
        if (field != null) {
            return GenerateGetterInner(field);
        }

        throw new MissingMemberException($"Member: \"{memberName}\" does not exist in \"{typeof(T).FullName}\".");
    }
    private static GetDelegate GenerateGetterInner(FieldInfo field) {
        DynamicMethod dynamicMethod = new DynamicMethod(string.Concat("_Get", field.Name, "_"), field.FieldType, new Type[] { typeof(T), }, typeof(T), true);

        ILGenerator generator = dynamicMethod.GetILGenerator();
        if (field.IsStatic) {
            generator.Emit(OpCodes.Ldsfld, field);
        }
        else {
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldfld, field);
        }
        generator.Emit(OpCodes.Ret);

        return (GetDelegate)dynamicMethod.CreateDelegate(typeof(GetDelegate));
    }
    #endregion
}
