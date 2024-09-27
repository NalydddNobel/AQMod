using Aequus.Common.ContentTemplates.Generic;
using System.Linq;
using Terraria.GameContent.ItemDropRules;

namespace Aequus.Content.Fishing.Crates;

internal class InstancedCrateItem(UnifiedModCrate parent, string nameSuffix, int style) : InstancedModItem($"{parent.Name}{nameSuffix}", $"{parent.Texture}{nameSuffix}Item") {
    protected readonly UnifiedModCrate _parent = parent;
    private readonly int _style = style;

    public override void SetStaticDefaults() {
        ItemID.Sets.IsFishingCrate[Type] = true;
        ItemID.Sets.IsFishingCrateHardmode[Type] = Name.EndsWith("Hard");

        Item.ResearchUnlockCount = 10;
    }

    public override void SetDefaults() {
        Item.CloneDefaults(ItemID.JungleFishingCrate);
        Item.createTile = _parent.Type;
        Item.placeStyle = _style;
    }

    public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup) {
        itemGroup = ContentSamples.CreativeHelper.ItemGroup.Crates;
    }

    public override bool CanRightClick() {
        return true;
    }

    public override void ModifyItemLoot(ItemLoot itemLoot) {
        IItemDropRule[] rules = _parent.ModifyCrateLoot(this, itemLoot).ToArray();

        itemLoot.Add(ItemDropRule.AlwaysAtleastOneSuccess(rules));
    }
}
