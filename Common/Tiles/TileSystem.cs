using Aequus.Items.Equipment.Accessories.Informational.Calendar;
using Terraria.ModLoader;

namespace Aequus.Common.Tiles;

public class TileSystem : ModSystem {
    public override void ResetNearbyTileEffects() {
        CalendarTile.Nearby = false;
    }
}