using Aequus.Content.Items.Accessories.FallSpeedHorseshoe;

namespace Aequus.Content.Items.Accessories.Tinkers.QuadBalloonCombos;

public class HorseshoeCombination : ModItem {
    public override void SetDefaults() {
        Item.DefaultToAccessory();
        Item.rare = ItemRarityID.Orange;
        Item.value = Item.sellPrice(gold: 1, silver: 50);
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        player.noFallDmg = true;
        player.fireWalk = true;
        WeightedHorseshoe.ApplyEffects(Item, player, WeightedHorseshoe.MaxFallSpeedMultiplier, WeightedHorseshoe.DamagingFallSpeedThreshold);
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
