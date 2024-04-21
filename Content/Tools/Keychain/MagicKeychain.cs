namespace Aequus.Content.Tools.Keychain;

internal class MagicKeychain : Keychain {
    public override void SetDefaults() {
        Item.width = 16;
        Item.height = 16;
        Item.rare = ItemRarityID.Cyan;
        Item.value = Item.sellPrice(gold: 5);
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
