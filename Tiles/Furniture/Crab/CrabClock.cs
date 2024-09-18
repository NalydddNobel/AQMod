namespace Aequus.Tiles.Furniture.Crab;
public class CrabClock : ModItem {
    public override void SetDefaults() {
        Item.DefaultToPlaceableTile(ModContent.TileType<CrabClockTile>());
        Item.value = Item.buyPrice(silver: 50);
    }
}

public class CrabClockTile : BaseWallClock {
}