using Aequus.Core.Graphics.GameOverlays;
using Terraria.Graphics.Effects;

namespace Aequus.Content.Graphics.GameOverlays;

public sealed class DrawsOverTilesNPCs : OverlayDrawLayer {
    public DrawsOverTilesNPCs() : base(EffectPriority.Medium, RenderLayers.ForegroundWater) {
    }
}
