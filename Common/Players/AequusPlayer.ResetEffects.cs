using Aequus.Common.Players.Attributes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Terraria;

namespace Aequus;

public partial class AequusPlayer {
    private static List<Action<AequusPlayer>> _resetEffectsAutomatic;

    private void AddResetEffects<T>(FieldInfo field) {
        string methodName = field.ReflectedType.FullName + ".set_" + field.Name;
        DynamicMethod setterMethod = new(methodName, null, new Type[] { typeof(AequusPlayer), typeof(T) }, true);
        ILGenerator gen = setterMethod.GetILGenerator();

        gen.Emit(OpCodes.Ldarg_0);
        gen.Emit(OpCodes.Ldarg_1);
        gen.Emit(OpCodes.Stfld, field);
        gen.Emit(OpCodes.Ret);

        var action = (Action<AequusPlayer, T>)setterMethod.CreateDelegate(typeof(Action<AequusPlayer, T>));
        _resetEffectsAutomatic.Add((aequusPlayer) => action(aequusPlayer, default(T)));
    }

    private void Load_AutomaticResetEffects() {
        _resetEffectsAutomatic = new();
        foreach (var f in GetType().GetFields(BindingFlags.Instance | BindingFlags.Public)) {
            if (f.GetCustomAttribute<ResetEffectsAttribute>() != null) {
                if (f.FieldType == typeof(bool)) {
                    AddResetEffects<bool>(f);
                }
                else if (f.FieldType == typeof(int)) {
                    AddResetEffects<int>(f);
                }
                else if (f.FieldType == typeof(float)) {
                    AddResetEffects<float>(f);
                }
                else if (f.FieldType == typeof(Item)) {
                    AddResetEffects<Item>(f);
                }
            }
        }
    }

    public override void ResetEffects() {
        if (Player.dashDelay == 0) {
            DashData = null;
        }
        for (int i = 0; i < _resetEffectsAutomatic.Count; i++) {
            _resetEffectsAutomatic[i](this);
        }
    }
}
