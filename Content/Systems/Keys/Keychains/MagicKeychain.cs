namespace Aequus.Content.Systems.Keys.Keychains;

internal class MagicKeychain : Keychain {
    public override void SetDefaults() {
        Item.width = 16;
        Item.height = 16;
        Item.CloneShopValues(ItemID.SpectrePaintbrush);
    }

    public override Color? GetAlpha(Color lightColor) {
        return Color.White with { A = 200 };
    }

    public override void UpdateInfoAccessory(Player player) {
        base.UpdateInfoAccessory(player);
        player.GetModPlayer<KeychainPlayer>().hasInfiniteKeyChain = true;
    }

    public override void AddRecipes() {
        CreateRecipe()
            .AddIngredient(ModContent.ItemType<Keychain>())
            .AddIngredient(ItemID.SpectreBar, 8)
            .AddTile(TileID.MythrilAnvil)
            .Register()
            .SortAfterFirstRecipesOf(ItemID.SpectrePaintScraper);
    }
}
