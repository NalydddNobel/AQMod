using System;
using tModLoaderExtended.Terraria.ModLoader;

namespace Aequus.Core.ContentGeneration;

[AttributeUsage(AttributeTargets.Class)]
internal class AutoloadBossBagAttribute(bool preHardmode, int itemRarity) : Attribute, IAttributeOnModLoad {
    public void OnModLoad(Mod mod, ILoad Content) {
        if (Content is not ModNPC modNPC) {
            throw new Exception($"{nameof(AutoloadBossBagAttribute)} can only be applied to ModNPCs.");
        }

        ModItem treasureBag = new InstancedBossBag(modNPC, itemRarity, preHardmode);
        mod.AddContent(treasureBag);
    }
}
