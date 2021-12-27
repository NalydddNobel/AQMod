using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace AQMod.Common.Graphics.SceneLayers
{
    public abstract class RenderTargetLayerType : SceneLayer
    {
        protected RenderTarget2D _finalTarget;

        public virtual bool ShouldReset()
        {
            return AQMod._lastScreenView != Main.ViewSize || AQMod._lastScreenZoom != new Vector2(Main.screenWidth, Main.screenHeight);
        }
        public abstract void ResetTargets(GraphicsDevice graphics);
        public abstract void DrawTargets();
        protected virtual void PreDrawFinal()
        {
        }
        protected virtual void PostDrawFinal()
        {
        }

        protected override void OnRegister(LayerKey key)
        {
            SceneLayersManager.RenderTargetLayers.RegisterRenderTargetLayer(Name, this, Layering);
        }

        protected override void Draw()
        {
            if (_finalTarget != null && !_finalTarget.IsContentLost)
            {
                PreDrawFinal();
                Main.spriteBatch.Draw(_finalTarget, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                PostDrawFinal();
            }
        }
    }
}