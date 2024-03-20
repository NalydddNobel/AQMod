namespace Aequus.Old.Content.Materials.MonoGem;

public class MonoGem : ModItem {
    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 25;
        ItemSets.SortingPriorityMaterials[Type] = ItemSets.SortingPriorityMaterials[ItemID.Amber];
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