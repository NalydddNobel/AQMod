using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Tiles.Conductive.CircuitBreaker;

public class CircuitBreakerTile : ModTile {
    public override string Texture => AequusTextures.Tile(TileID.Switches);

    public override void SetStaticDefaults() {
    }
}