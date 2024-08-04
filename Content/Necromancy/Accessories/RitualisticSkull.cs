namespace Aequus.Content.Necromancy.Accessories;

public class RitualisticSkull : ModItem {
    public override void SetDefaults() {
        Item.width = 24;
        Item.height = 24;
        Item.accessory = true;
        Item.rare = ItemRarityID.Yellow;
        Item.value = Item.sellPrice(gold: 10);
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        player.GetModPlayer<AequusPlayer>().accRitualSkull = true;
    }

    public override void AddRecipes() {
        CreateRecipe()
            .AddIngredient(ItemID.PygmyNecklace)
            .AddIngredient(ItemID.SpectreBar, 8)
            .AddIngredient(ItemID.SoulofFright, 8)
            .AddTile(TileID.MythrilAnvil)
            .Register()
            .SortAfterFirstRecipesOf(ItemID.PapyrusScarab);
    }
}