using Aequus.Common.Graphics.Rendering.Tiles;
using Aequus.Core.Graphics.GameOverlays;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Effects;

namespace Aequus.Content.Graphics.GameOverlays;

public sealed class TileOverWaterOverlay : AequusOverlay {
    private TileOverWaterOverlay() : base(EffectPriority.Medium, RenderLayers.ForegroundWater) {
    }

    public override bool SpecialVisuals(Player player) {
        return SpecialTileRenderer.AnyInLayer(TileRenderLayerID.PostDrawLiquids);
    }

    public override void Draw(SpriteBatch spriteBatch) {
        SpecialTileRenderer.Render(TileRenderLayerID.PostDrawLiquids);
    }
}