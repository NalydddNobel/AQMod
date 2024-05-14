namespace Aequus.Content.Items.Accessories.EventPrevention;

[AutoloadEquip(EquipType.Face)]
public class EclipseGlasses : ModItem {
    public override void SetDefaults() {
        Item.DefaultToAccessory();
        Item.rare = ItemRarityID.Cyan;
        Item.value = Item.sellPrice(gold: 2);
    }

    public override void UpdateEquip(Player player) {
        player.GetModPlayer<EventDeactivatorPlayer>().accDisableEclipse = true;
    }

    public override void AddRecipes() {
        CreateRecipe()
            .AddIngredient(ItemID.BlackLens)
            .AddIngredient(ItemID.LunarTabletFragment, 5)
            .AddIngredient(ItemID.Ectoplasm, 5)
            .AddTile(TileID.MythrilAnvil)
            .Register();
    }
}
