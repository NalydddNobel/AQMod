using Aequu2.Old.Content.Events.Glimmer.CosmicMonolith;

namespace Aequu2.Old.Content.Tiles;

public class MonolithsSystem : ModSystem {
    public override void ResetNearbyTileEffects() {
        CosmicMonolithScene.MonolithNearby = false;
    }
}