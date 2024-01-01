using Microsoft.Xna.Framework.Graphics;

namespace Aequus.Core.Graphics;

public abstract class ScreenRenderer : RequestRenderer {
    public virtual int FinalResultResolutionDiv => 1;

    protected override void PrepareRenderTargetsForDrawing(GraphicsDevice device, SpriteBatch spriteBatch) {
        PrepareARenderTarget_AndListenToEvents(ref _target, device, Main.screenWidth / FinalResultResolutionDiv, Main.screenHeight / FinalResultResolutionDiv, RenderTargetUsage.PreserveContents);
        PrepareARenderTarget_WithoutListeningToEvents(ref helperTarget, device, Main.screenWidth, Main.screenHeight, RenderTargetUsage.DiscardContents);
    }
}