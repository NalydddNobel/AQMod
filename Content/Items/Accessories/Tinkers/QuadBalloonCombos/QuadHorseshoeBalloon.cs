using Aequus.Content.Items.Accessories.Balloons;

namespace Aequus.Content.Items.Accessories.Tinkers.QuadBalloonCombos;

[AutoloadEquip(EquipType.Balloon)]
public class QuadHorseshoeBalloon : ModItem {
    public override void SetDefaults() {
        int balloonSlot = Item.balloonSlot;
        Item.CloneDefaults(ItemID.HorseshoeBundle);
        Item.balloonSlot = balloonSlot;
        Item.rare = ItemRarityID.Cyan;
        Item.value = (int)(Item.value * 1.1f);
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        player.ApplyEquipFunctional(ContentSamples.ItemsByType[ItemID.HorseshoeBundle], hideVisual);
        SlimyBlueBalloon.ApplyEffects(player, SlimyBlueBalloon.MaxFallSpeedMultiplier, SlimyBlueBalloon.FallGravityMultiplier);
    }

    public override void AddRecipes() {
        CreateRecipe()
            .AddIngredient<QuadBalloon>()
            .AddIngredient(ItemID.LuckyHorseshoe)
            .AddTile(TileID.TinkerersWorkbench)
            .Register();

        CreateRecipe()
            .AddIngredient(ItemID.HorseshoeBundle)
            .AddIngredient<SlimyBlueBalloon>()
            .AddTile(TileID.TinkerersWorkbench)
            .Register();

        CreateRecipe()
            .AddIngredient(ItemID.HorseshoeBundle)
            .AddIngredient<PurpleBalloon>()
            .AddTile(TileID.TinkerersWorkbench)
            .Register();
    }
}