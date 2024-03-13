using Aequus.Old.Content.Events.Glimmer.CosmicMonolith;

namespace Aequus.Old.Content.Tiles;

public class MonolithsSystem : ModSystem {
    public override void ResetNearbyTileEffects() {
        CosmicMonolithScene.MonolithNearby = false;
    }
}