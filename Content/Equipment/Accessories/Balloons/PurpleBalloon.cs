namespace Aequus.Content.Equipment.Accessories.Balloons;

[AutoloadEquip(EquipType.Balloon)]
public class PurpleBalloon : SlimyBlueBalloon {
    public override void SetDefaults() {
        System.Int32 balloonSlot = Item.balloonSlot;
        Item.CloneDefaults(ItemID.ShinyRedBalloon);
        Item.balloonSlot = balloonSlot;
        Item.rare++;
        Item.value = (System.Int32)(Item.value * 1.5f);
    }

    public override void UpdateAccessory(Player player, System.Boolean hideVisual) {
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