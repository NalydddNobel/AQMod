using Aequus.Content.Items.Material;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Tiles.CrabPots;

public class CrabPotTinItem : ModItem {
    public override void SetDefaults() {
        Item.DefaultToPlaceableTile(ModContent.TileType<CrabPot>(), tileStyleToPlace: CrabPot.TinPot);
        Item.rare = ItemRarityID.Blue;
        Item.value = Item.sellPrice(silver: 20);
    }

    public override void AddRecipes() {
        CreateRecipe()
            .AddIngredient(ItemID.TinBar, 10)
            .AddIngredient(ItemID.Chain, 3)
            .AddIngredient<CompressedTrash>()
            .AddTile(TileID.Anvils)
            .Register();
    }
}