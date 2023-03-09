using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Aequus.Common.Effects.RenderBatches
{
    public class BehindAllNPCsNoWorldScaleBatch : RenderLayerBatch
    {
        public override void Begin(SpriteBatch spriteBatch)
        {
            spriteBatch.End();
            spriteBatch.Begin_World(shader: false, Matrix.Identity);
        }

        public override void End(SpriteBatch spriteBatch)
        {
            spriteBatch.End();
            spriteBatch.Begin_World(shader: false);
        }
    }
}