namespace Aequus.Content.Items.Accessories.WeightedHorseshoe;

public class HorseshoeCombination : WeightedHorseshoe {
    public override void SetDefaults() {
        Item.DefaultToAccessory();
        Item.rare = ItemRarityID.Orange;
        Item.value = Item.sellPrice(gold: 1, silver: 50);
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        player.noFallDmg = true;
        player.fireWalk = true;
        base.UpdateAccessory(player, hideVisual);
    }

    public override void AddRecipes() {
        CreateRecipe()
            .AddIngredient(ItemID.ObsidianHorseshoe)
            .AddIngredient(ModContent.ItemType<WeightedHorseshoe>())
            .AddTile(TileID.TinkerersWorkbench)
            .Register()
            .SortBeforeFirstRecipesOf(ItemID.ObsidianHorseshoe);
    }
}
