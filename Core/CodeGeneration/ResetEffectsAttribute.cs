using System;

namespace Aequus.Core.CodeGeneration;

[AttributeUsage(AttributeTargets.Field)]
[Obsolete("Replaced with PlayerGen/NPCGen.ResetEffectsAttribute")]
public sealed class ResetEffectsAttribute : Attribute {
    internal object resetValue;

    public ResetEffectsAttribute() : this(resetValue: null) { }

    public ResetEffectsAttribute(object resetValue = null) {
        this.resetValue = resetValue;
    }
}