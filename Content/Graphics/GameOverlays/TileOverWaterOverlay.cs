using Aequus.Core.Graphics.GameOverlays;
using Aequus.Core.Graphics.Tiles;
using Terraria.Graphics.Effects;

namespace Aequus.Content.Graphics.GameOverlays;

public sealed class TileOverWaterOverlay : AequusOverlay {
    private TileOverWaterOverlay() : base(EffectPriority.Medium, RenderLayers.ForegroundWater) {
    }

    public override System.Boolean SpecialVisuals(Player player) {
        return SpecialTileRenderer.AnyInLayer(TileRenderLayerID.PostDrawLiquids);
    }

    public override void Draw(SpriteBatch spriteBatch) {
        SpecialTileRenderer.Render(TileRenderLayerID.PostDrawLiquids);
    }
}