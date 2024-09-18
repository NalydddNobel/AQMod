namespace Aequus.Tiles.CraftingStations;

public class OblivionCraftingStationItem : ModItem {
    public override void SetDefaults() {
        Item.DefaultToPlaceableTile(ModContent.TileType<OblivionCraftingStationTile>());
        Item.value = Item.buyPrice(gold: 5);
        Item.rare = ItemRarityID.White;
    }
}