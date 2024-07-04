using AequusRemake.Content.Items.Accessories.Informational.Calendar;

namespace AequusRemake.Core.Entities.Tiles;

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