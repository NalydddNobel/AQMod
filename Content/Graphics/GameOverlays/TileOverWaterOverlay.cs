using Aequu2.Core.Graphics.GameOverlays;
using Aequu2.Core.Graphics.Tiles;
using Terraria.Graphics.Effects;

namespace Aequu2.Content.Graphics.GameOverlays;

public sealed class TileOverWaterOverlay : Aequu2Overlay {
    private TileOverWaterOverlay() : base(EffectPriority.Medium, RenderLayers.ForegroundWater) {
    }

    public override bool SpecialVisuals(Player player) {
        return SpecialTileRenderer.AnyInLayer(TileRenderLayerID.PostDrawLiquids);
    }

    public override void Draw(SpriteBatch spriteBatch) {
        SpecialTileRenderer.Render(TileRenderLayerID.PostDrawLiquids);
    }
}