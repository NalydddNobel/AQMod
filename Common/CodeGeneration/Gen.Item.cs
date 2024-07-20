using System;

namespace Aequus.Common.CodeGeneration;

internal partial class Gen {
    /// <summary>Adds a reference to the target method in <see cref="AequusItem.UseItem"/>.</summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    internal class AequusItem_UseItem : Attribute { }
    /// <summary>Adds a reference to the target method in <see cref="AequusItem.ModifyTooltips(Item, System.Collections.Generic.List{TooltipLine})"/>.</summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    internal class AequusItem_ModifyTooltips : Attribute { }
}
