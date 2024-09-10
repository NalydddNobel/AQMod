using Aequus.Content.Items.Accessories.Balloons;
using Aequus.Content.Items.Accessories.FallSpeedHorseshoe;

namespace Aequus.Content.Items.Accessories.Tinkers.QuadBalloonCombos;

[AutoloadEquip(EquipType.Balloon)]
public class TerraBalloon : ModItem {
    public override void SetDefaults() {
        int balloonSlot = Item.balloonSlot;
        Item.CloneDefaults(ItemID.HorseshoeBundle);
        Item.balloonSlot = balloonSlot;
        Item.rare = ItemRarityID.Red;
        Item.value = (int)(Item.value * 1.25f);
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        // Horseshoe bundle effect.
        player.ApplyEquipFunctional(ContentSamples.ItemsByType[ItemID.HorseshoeBundle], hideVisual);

        // Obsidian Skull effect.
        player.fireWalk = true;

        // Slimy Balloon effect. (Only when holding UP.)
        if (player.controlUp) {
            SlimyBlueBalloon.ApplyEffects(player, SlimyBlueBalloon.MaxFallSpeedMultiplier, SlimyBlueBalloon.FallGravityMultiplier);
        }
        // Weighted Horseshoe effect. (Only when holding DOWN.)
        else if (player.controlDown) {
            WeightedHorseshoe.ApplyEffects(Item, player, WeightedHorseshoe.MaxFallSpeedMultiplier, WeightedHorseshoe.DamagingFallSpeedThreshold);
        }
    }

    public override void AddRecipes() {
        CreateRecipe()
            .AddIngredient<QuadBalloon>()
            .AddIngredient<HorseshoeCombination>()
            .AddTile(TileID.TinkerersWorkbench)
            .Register();

        CreateRecipe()
            .AddIngredient<QuadHorseshoeBalloon>()
            .AddIngredient(ItemID.ObsidianSkull)
            .AddIngredient<WeightedHorseshoe>()
            .AddTile(TileID.TinkerersWorkbench)
            .Register();

        CreateRecipe()
            .AddIngredient<QuadHorseshoeBalloon>()
            .AddIngredient<HorseshoeCombination>()
            .AddTile(TileID.TinkerersWorkbench)
            .Register();
    }
}