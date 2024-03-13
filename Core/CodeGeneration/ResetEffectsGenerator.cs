using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Aequus.Core.CodeGeneration;

public sealed class ResetEffectsGenerator<T> {
    private Action<T> ResetEffects;

    public void Generate() {
        var generatedMethod = new DynamicMethod("ResetEffects", null, new Type[] { typeof(T), }, true);
        var gen = generatedMethod.GetILGenerator();

        foreach (var f in typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public)) {
            var attr = f.GetCustomAttribute<ResetEffectsAttribute>();
            if (attr == null) {
                continue;
            }

            gen.Emit(OpCodes.Ldarg_0);
            if (f.FieldType == typeof(bool)) {
                gen.Emit((bool)(attr.resetValue ?? false) ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
            }
            else if (f.FieldType == typeof(int)) {
                gen.Emit(OpCodes.Ldc_I4, (int)(attr.resetValue ?? 0));
            }
            else if (f.FieldType == typeof(float)) {
                gen.Emit(OpCodes.Ldc_R4, (float)(attr.resetValue ?? 0f));
            }
            else if (f.FieldType == typeof(StatModifier)) {
                gen.Emit(OpCodes.Ldsfld, typeof(StatModifier).GetField(nameof(StatModifier.Default), BindingFlags.Public | BindingFlags.Static));
            }
            else if (!f.FieldType.IsValueType || Nullable.GetUnderlyingType(f.FieldType) != null) {
                gen.Emit(OpCodes.Ldnull);
            }
            else {
                throw new Exception($"Generator: Could not find load operation for Type '{f.FieldType.FullName}'");
            }
            gen.Emit(OpCodes.Stfld, f);
        }
        gen.Emit(OpCodes.Ret);

        ResetEffects = (Action<T>)generatedMethod.CreateDelegate(typeof(Action<T>));
    }

    public void Invoke(T instance) {
        ResetEffects(instance);
    }
}