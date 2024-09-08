using Aequus.Content.Items.Accessories.Balloons;

namespace Aequus.Content.Items.Accessories.Tinkers.PurpleBalloon;

[AutoloadEquip(EquipType.Balloon)]
public class PurpleBalloon : ModItem {
    public override void SetDefaults() {
        int balloonSlot = Item.balloonSlot;
        Item.CloneDefaults(ItemID.ShinyRedBalloon);
        Item.balloonSlot = balloonSlot;
        Item.rare++;
        Item.value = (int)(Item.value * 1.25f);
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        player.jumpBoost = true;
        SlimyBlueBalloon.ApplyEffects(player, SlimyBlueBalloon.MaxFallSpeedMultiplier, SlimyBlueBalloon.FallGravityMultiplier);
    }

    public override void AddRecipes() {
        CreateRecipe()
            .AddIngredient(ItemID.ShinyRedBalloon)
            .AddIngredient<SlimyBlueBalloon>()
            .AddTile(TileID.TinkerersWorkbench)
            .Register();
    }
}