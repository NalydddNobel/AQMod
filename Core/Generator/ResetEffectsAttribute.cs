using System;

namespace Aequus.Core.Generator;

[AttributeUsage(AttributeTargets.Field)]
public sealed class ResetEffectsAttribute : Attribute {
    internal object resetValue;

    public ResetEffectsAttribute(object resetValue = null) {
        this.resetValue = resetValue;
    }
}