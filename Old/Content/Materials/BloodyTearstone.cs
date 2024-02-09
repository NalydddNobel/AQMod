using Aequus.Common.Items;

namespace Aequus.Old.Content.Materials;

[LegacyName("BloodyTearFragment")]
public class BloodyTearstone : ModItem {
    public override void SetStaticDefaults() {
        ItemSets.SortingPriorityMaterials[Type] = ItemSortingPriority.Materials.DemoniteOre;
        Item.ResearchUnlockCount = 25;
    }

    public override void SetDefaults() {
        Item.width = 12;
        Item.height = 12;
        Item.maxStack = Item.CommonMaxStack;
        Item.rare = ItemRarityID.Blue;
        Item.value = Item.sellPrice(silver: 2);
    }
}