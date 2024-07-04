using AequusRemake.Core.Graphics.GameOverlays;
using Terraria.Graphics.Effects;

namespace AequusRemake.Content.Graphics.GameOverlays;

public sealed class DrawsOverTilesNPCs : OverlayDrawLayer {
    public DrawsOverTilesNPCs() : base(EffectPriority.Medium, RenderLayers.ForegroundWater) {
    }
}
