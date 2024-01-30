namespace Aequus.Content.Equipment.Accessories.Balloons;

[AutoloadEquip(EquipType.Balloon)]
public class PurpleBalloon : SlimyBlueBalloon {
    public override void SetDefaults() {
        int balloonSlot = Item.balloonSlot;
        Item.CloneDefaults(ItemID.ShinyRedBalloon);
        Item.balloonSlot = balloonSlot;
        Item.rare++;
        Item.value = (int)(Item.value * 1.5f);
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        player.jumpBoost = true;
        base.UpdateAccessory(player, hideVisual);
    }

    public override void AddRecipes() {
        CreateRecipe()
            .AddIngredient(ItemID.ShinyRedBalloon)
            .AddIngredient<SlimyBlueBalloon>()
            .AddTile(TileID.TinkerersWorkbench)
            .Register();
    }
}