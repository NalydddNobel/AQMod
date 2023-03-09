using Aequus.Common.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Aequus.Common.Rendering
{
    public abstract class RequestableRenderTarget : ARenderTargetContentByRequest, ILoadable
    {
        protected RenderTarget2D helperTarget;

        protected virtual void PrepareRenderTargetsForDrawing(GraphicsDevice device, SpriteBatch spriteBatch)
        {
        }

        protected override void HandleUseReqest(GraphicsDevice device, SpriteBatch spriteBatch)
        {
            PrepareRenderTargetsForDrawing(device, spriteBatch);

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

        /// <summary>
        /// Ran on world Load/Unload
        /// </summary>
        public virtual void CleanUp()
        {
        }

        public virtual void Load(Mod mod)
        {
            LegacyEffects.Renderers ??= new List<RequestableRenderTarget>();
            LegacyEffects.Renderers.Add(this);
        }
        public virtual void Unload()
        {
            Main.QueueMainThreadAction(() =>
            {
                DisposeResources();
            });
        }
        public virtual void DisposeResources()
        {
            helperTarget?.Dispose();
            helperTarget = null;
            _target?.Dispose();
            _target = null;
        }

        public void CheckSelfRequest()
        {
            if (PrepareTarget())
            {
                Request();
            }
        }

        protected virtual bool PrepareTarget()
        {
            return false;
        }

        protected bool IsTargetUsable(RenderTarget2D target)
        {
            return target != null && !target.IsDisposed && !target.IsContentLost;
        }
    }
}