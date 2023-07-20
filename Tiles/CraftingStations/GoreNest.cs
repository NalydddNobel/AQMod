using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Tiles.CraftingStations;

public class GoreNest : ModItem {
    public override void SetDefaults() {
        Item.DefaultToPlaceableTile(ModContent.TileType<GoreNestTile>());
        Item.value = Item.buyPrice(gold: 5);
        Item.rare = ItemRarityID.Orange;
    }
}