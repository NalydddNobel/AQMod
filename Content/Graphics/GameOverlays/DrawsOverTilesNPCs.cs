using Aequu2.Core.Graphics.GameOverlays;
using Terraria.Graphics.Effects;

namespace Aequu2.Content.Graphics.GameOverlays;

public sealed class DrawsOverTilesNPCs : OverlayDrawLayer {
    public DrawsOverTilesNPCs() : base(EffectPriority.Medium, RenderLayers.ForegroundWater) {
    }
}
