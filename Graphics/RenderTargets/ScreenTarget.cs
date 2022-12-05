using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Aequus.Graphics.RenderTargets
{
    public abstract class ScreenTarget : RequestableRenderTarget
    {
        public virtual int FinalResultResolutionDiv => 1;

        protected override void PrepareRenderTargetsForDrawing(GraphicsDevice device, SpriteBatch spriteBatch)
        {
            PrepareARenderTarget_AndListenToEvents(ref _target, device, Main.screenWidth / FinalResultResolutionDiv, Main.screenHeight / FinalResultResolutionDiv, RenderTargetUsage.PreserveContents);
            PrepareARenderTarget_WithoutListeningToEvents(ref helperTarget, device, Main.screenWidth, Main.screenHeight, RenderTargetUsage.DiscardContents);
        }
    }
}