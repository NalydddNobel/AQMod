using Aequus.Common.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Items.Material.MonoGem;

public class MonoGem : ModItem {
    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 25;
        ItemID.Sets.SortingPriorityMaterials[Type] = ItemSortingPriority.Materials.Amber;
    }

    public override void SetDefaults() {
        Item.DefaultToPlaceableTile(ModContent.TileType<MonoGemTile>());
        Item.rare = ItemRarityID.Green;
        Item.value = Item.sellPrice(silver: 2);
    }

    public override void AddRecipes() {
        //for (int i = 0; i < ItemLoader.ItemCount; i++) {
        //    if (ItemID.Sets.Deprecated[i]) {
        //        continue;
        //    }

        //    int shimmerItemID = i;
        //    if (ItemID.Sets.ShimmerCountsAsItem[i] > -1) {
        //        shimmerItemID = ItemID.Sets.ShimmerCountsAsItem[i];
        //    }
        //    if (ItemID.Sets.ShimmerTransformToItem[shimmerItemID] <= 0) {
        //        continue;
        //    }
        //    Recipe.Create(i)
        //        .AddIngredient(ItemID.Sets.ShimmerTransformToItem[shimmerItemID])
        //        .AddIngredient(Type, 5)
        //        .Register();
        //}
    }
}