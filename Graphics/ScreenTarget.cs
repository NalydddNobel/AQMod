using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Aequus.Graphics
{
    public abstract class ScreenTarget : ARenderTargetContentByRequest, ILoadable
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

        public virtual void Load(Mod mod)
        {
            if (AequusEffects.Renderers == null)
                AequusEffects.Renderers = new List<ScreenTarget>();
            AequusEffects.Renderers.Add(this);
        }
        public virtual void Unload()
        {

        }

        public void CheckSelfRequest()
        {
            if (SelfRequest())
            {
                Request();
            }
        }

        protected virtual bool SelfRequest()
        {
            return false;
        }
    }
}