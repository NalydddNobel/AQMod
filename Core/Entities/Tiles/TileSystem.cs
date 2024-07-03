using Aequu2.Content.Items.Accessories.Informational.Calendar;

namespace Aequu2.Core.Entities.Tiles;

public class TileSystem : ModSystem {
    public override void ResetNearbyTileEffects() {
        CalendarTile.Nearby = false;
    }

    public override void ClearWorld() {
        MultiMergeTile.EnsureCacheLength(MultiMergeTile.DummyEntry);
    }

    public override void Unload() {
    }
}