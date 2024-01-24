using Aequus.Core.Initialization;
using System;

namespace Aequus.Content.Tiles.Statues;

internal class AutoloadStatueAttribute : AutoloadXAttribute {
    internal override void Load(ModType modType) {
        if (modType is not ModNPC modNPC) {
            throw new Exception("AutoloadBannerAttribute can only be applied to ModNPCs.");
        }

        modNPC.Mod.AddContent(new InstancedNPCStatue(modNPC));
    }
}
