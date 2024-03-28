using Aequus.Content.Bosses;
using Aequus.Core.Initialization;
using System;

namespace Aequus.Core.ContentGeneration;

internal class AutoloadBossMaskAttribute : AutoloadXAttribute {
    internal override void Load(ModType modType) {
        if (modType is not ModNPC modNPC) {
            throw new Exception($"{nameof(AutoloadBossMaskAttribute)} can only be applied to ModNPCs.");
        }

        ModItem bossMask = new InstancedBossMask(modNPC);
        modType.Mod.AddContent(bossMask);
    }
}
