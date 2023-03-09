using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace Aequus.Common.Effects.RenderBatches
{
    public abstract class RenderLayerBatch : ILoadable
    {
        internal List<ILayerRenderer> renderers = new();

        public void FullRender(SpriteBatch spriteBatch)
        {
            foreach (var r in renderers)
            {
                if (r.IsReady)
                {
                    goto DoRender;
                }
            }

            return;

        DoRender:
            Begin(spriteBatch);
            Render(spriteBatch);
            End(spriteBatch);
        }

        public abstract void Begin(SpriteBatch spriteBatch);

        public virtual void Render(SpriteBatch spriteBatch)
        {
            foreach (var r in renderers)
            {
                r.DrawToLayer(this, spriteBatch);
            }
        }

        public abstract void End(SpriteBatch spriteBatch);

        public void Load(Mod mod)
        {
            OnLoad();
        }

        public virtual void OnLoad()
        {
        }

        public void Unload()
        {
            renderers.Clear();
            OnUnload();
        }

        public virtual void OnUnload()
        {
        }
    }
}