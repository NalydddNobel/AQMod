using Aequus.Content.Bosses;
using Aequus.Core.Initialization;
using System;

namespace Aequus.Core.ContentGeneration;

internal class AutoloadBossBagAttribute : AutoloadXAttribute {
    private readonly bool _preHardmode;
    private readonly int _itemRarity;

    public AutoloadBossBagAttribute(bool preHardmode, int itemRarity) {
        _preHardmode = preHardmode;
        _itemRarity = itemRarity;
    }

    internal override void Load(ModType modType) {
        if (modType is not ModNPC modNPC) {
            throw new Exception($"{nameof(AutoloadBossBagAttribute)} can only be applied to ModNPCs.");
        }

        ModItem treasureBag = new InstancedBossBag(modNPC, _itemRarity, _preHardmode);
        modType.Mod.AddContent(treasureBag);
    }
}
