namespace Aequus.Tiles.CraftingStations;

public class RecyclingMachine : ModItem {
    public override void SetDefaults() {
        Item.DefaultToPlaceableTile(ModContent.TileType<RecyclingMachineTile>());
        Item.value = Item.buyPrice(gold: 2, silver: 50);
        Item.rare = ItemRarityID.Blue;
    }
}