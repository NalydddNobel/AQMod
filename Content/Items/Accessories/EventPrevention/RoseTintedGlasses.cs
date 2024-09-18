namespace Aequus.Content.Items.Accessories.EventPrevention;

[AutoloadEquip(EquipType.Face)]
public class RoseTintedGlasses : ModItem {
    public override void SetDefaults() {
        Item.DefaultToAccessory();
        Item.rare = ItemRarityID.Cyan;
        Item.value = Item.sellPrice(gold: 2);
    }

    public override void UpdateEquip(Player player) {
        player.GetModPlayer<EventDeactivatorPlayer>().accDisableBloodMoon = true;
    }

    public override void AddRecipes() {
        CreateRecipe()
            .AddIngredient(ItemID.BlackLens)
            .AddIngredient<global::Aequus.Items.Materials.BloodyTearstone>(5)
            .AddTile(TileID.Anvils)
            .Register();
    }
}
