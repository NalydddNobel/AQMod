using Aequus.Core.Initialization;
using System;

namespace Aequus.Core.ContentGeneration;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
internal sealed class AutoloadCatchItemAttribute : AutoloadXAttribute {
    public AutoloadCatchItemAttribute() { }

    internal override void Load(ModType modType) {
        if (modType is not ModNPC modNPC) {
            throw new Exception($"{nameof(AutoloadCatchItemAttribute)} can only be applied to ModNPCs.");
        }

        modType.Mod.AddContent(new InstancedCaughtNPCItem(modNPC));
    }

    // Setting the catch item is handled in AutoNPCDefaults.SetDefaults
}
