using AequusRemake.Core.Graphics.GameOverlays;
using AequusRemake.Core.Graphics.Tiles;
using Terraria.Graphics.Effects;

namespace AequusRemake.Content.Graphics.GameOverlays;

public sealed class TileOverWaterOverlay : AequusRemakeOverlay {
    private TileOverWaterOverlay() : base(EffectPriority.Medium, RenderLayers.ForegroundWater) {
    }

    public override bool SpecialVisuals(Player player) {
        return SpecialTileRenderer.AnyInLayer(TileRenderLayerID.PostDrawLiquids);
    }

    public override void Draw(SpriteBatch spriteBatch) {
        SpecialTileRenderer.Render(TileRenderLayerID.PostDrawLiquids);
    }
}