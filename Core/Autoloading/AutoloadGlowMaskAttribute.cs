using System;

namespace Aequus.Core.Autoloading;

[AttributeUsage(AttributeTargets.Class)]
internal class AutoloadGlowMaskAttribute : Attribute {
    public readonly string[] CustomGlowmasks;
    public readonly bool AutoAssignItemID;

    public AutoloadGlowMaskAttribute() {
        AutoAssignItemID = true;
        CustomGlowmasks = null;
    }

    public AutoloadGlowMaskAttribute(params string[] glowmasks) {
        AutoAssignItemID = false;
        CustomGlowmasks = glowmasks;
    }
}