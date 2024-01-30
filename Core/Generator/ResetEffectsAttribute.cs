using System;

namespace Aequus.Core.Generator;

[AttributeUsage(AttributeTargets.Field)]
public sealed class ResetEffectsAttribute : Attribute {
    internal Object resetValue;

    public ResetEffectsAttribute(Object resetValue = null) {
        this.resetValue = resetValue;
    }
}