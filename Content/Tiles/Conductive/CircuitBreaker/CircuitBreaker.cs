using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Tiles.Conductive.CircuitBreaker;

public class CircuitBreaker : ModItem {
    public override string Texture => AequusTextures.Item(ItemID.Switch);

    public override void SetDefaults() {
        Item.DefaultToPlaceableTile(ModContent.TileType<CircuitBreakerTile>());
        Item.color = Colors.RarityOrange;
    }
}