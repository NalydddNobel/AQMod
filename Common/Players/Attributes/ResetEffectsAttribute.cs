using System;

namespace Aequus.Common.Players.Attributes;

public class ResetEffectsAttribute : Attribute {
    internal object resetValue;

    public ResetEffectsAttribute(object resetValue = null) {
        this.resetValue = resetValue;
    }
}