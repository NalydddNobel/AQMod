using System;

namespace Aequus.Core.Generator;

/// <summary>
/// Supports:
/// <list type="bullet">
/// <item><see cref="bool"/></item>
/// <item><see cref="int"/></item>
/// <item><see cref="int"/>?</item>
/// <item><see cref="float"/></item>
/// <item><see cref="float"/>?</item>
/// <item><see cref="Terraria.Item"/></item>
/// </list>
/// </summary>
public class ResetEffectsAttribute : Attribute {
    internal object resetValue;

    public ResetEffectsAttribute(object resetValue = null) {
        this.resetValue = resetValue;
    }
}