using Aequus.Common.Players.Attributes;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Terraria;

namespace Aequus.Core.Generator;

public class ResetEffectsGenerator<T> {
    private List<Action<T>> _list;

    public ResetEffectsGenerator() {
        _list = new();
    }

    public void Generate() {
        foreach (var f in typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public)) {
            var attr = f.GetCustomAttribute<ResetEffectsAttribute>();
            if (attr != null) {
                if (f.FieldType == typeof(bool)) {
                    AddResetEffects<bool>(f, attr);
                }
                else if (f.FieldType == typeof(int)) {
                    AddResetEffects<int>(f, attr);
                }
                else if (f.FieldType == typeof(float)) {
                    AddResetEffects<float>(f, attr);
                }
                else if (f.FieldType == typeof(Item)) {
                    AddResetEffects<Item>(f, attr);
                }
            }
        }
    }

    private void AddResetEffects<T2>(FieldInfo field, ResetEffectsAttribute attribute) {
        var action = CreateResetAction<T2>(field.ReflectedType.FullName + ".set_" + field.Name, field);

        var value = (T2)(attribute?.resetValue ?? default(T2));
        _list.Add((val) => action(val, value));
    }

    public void Invoke(T instance) {
        for (int i = 0; i < _list.Count; i++) {
            _list[i](instance);
        }
    }

    public static Action<T, T2> CreateResetAction<T2>(string methodName, FieldInfo fieldInfo) {
        DynamicMethod setterMethod = new(methodName, null, new Type[] { typeof(T), typeof(T2) }, true);
        ILGenerator gen = setterMethod.GetILGenerator();

        gen.Emit(OpCodes.Ldarg_0);
        gen.Emit(OpCodes.Ldarg_1);
        gen.Emit(OpCodes.Stfld, fieldInfo);
        gen.Emit(OpCodes.Ret);

        return (Action<T, T2>)setterMethod.CreateDelegate(typeof(Action<T, T2>));
    }
}