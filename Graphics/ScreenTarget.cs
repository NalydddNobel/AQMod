using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;

namespace Aequus.Graphics
{
    public abstract class ScreenTarget : ARenderTargetContentByRequest
    {
        protected RenderTarget2D helperTarget;

        protected override void HandleUseReqest(GraphicsDevice device, SpriteBatch spriteBatch)
        {
            PrepareARenderTarget_AndListenToEvents(ref _target, device, Main.screenWidth, Main.screenHeight, RenderTargetUsage.PreserveContents);
            PrepareARenderTarget_WithoutListeningToEvents(ref helperTarget, device, Main.screenWidth, Main.screenHeight, RenderTargetUsage.DiscardContents);

            var targets = device.GetRenderTargets();
            device.SetRenderTarget(helperTarget);
            device.Clear(Color.Transparent);
            DrawOntoTarget(device, spriteBatch);
            device.SetRenderTargets(targets);
        }

        protected virtual void DrawOntoTarget(GraphicsDevice device, SpriteBatch spriteBatch)
        {
        }

        protected void DrawHelperToTarget(RenderTarget2D target, GraphicsDevice device, SpriteBatch spriteBatch, Action action)
        {
            device.SetRenderTarget(target);

            action();

            device.SetRenderTarget(helperTarget);
        }
    }
}