using Aequus.Tiles.Monoliths.CosmicMonolith;
using Terraria.ModLoader;

namespace Aequus.Tiles.Monoliths;

public class MonolithsSystem : ModSystem {
    public override void ResetNearbyTileEffects() {
        CosmicMonolithScene.MonolithNearby = false;
    }
}