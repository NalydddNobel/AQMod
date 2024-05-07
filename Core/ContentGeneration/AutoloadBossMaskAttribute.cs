using System;
using tModLoaderExtended.Terraria.ModLoader;

namespace Aequus.Core.ContentGeneration;

internal class AutoloadBossMaskAttribute : Attribute, IAttributeOnModLoad {
    public void OnModLoad(Mod mod, ILoad Content) {
        if (Content is not ModNPC modNPC) {
            throw new Exception($"{nameof(AutoloadBossMaskAttribute)} can only be applied to ModNPCs.");
        }

        ModItem bossMask = new InstancedBossMask(modNPC);
        mod.AddContent(bossMask);
    }
}
