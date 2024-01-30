using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Aequus.Core.Generator;

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
            if (f.FieldType == typeof(Boolean)) {
                gen.Emit((Boolean)(attr.resetValue ?? false) ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
            }
            else if (f.FieldType == typeof(Int32)) {
                gen.Emit(OpCodes.Ldc_I4, (Int32)(attr.resetValue ?? 0));
            }
            else if (f.FieldType == typeof(Single)) {
                gen.Emit(OpCodes.Ldc_R4, (Single)(attr.resetValue ?? 0f));
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