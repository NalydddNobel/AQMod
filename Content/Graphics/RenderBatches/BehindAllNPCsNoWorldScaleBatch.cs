using Aequus.Common.Graphics.RenderBatches;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Aequus.Content.Graphics.RenderBatches {
    public class BehindAllNPCsNoWorldScaleBatch : RenderLayerBatch {
        public override void Begin(SpriteBatch spriteBatch) {
            spriteBatch.End();
            spriteBatch.Begin_World(shader: false, Matrix.Identity);
        }

        public override void End(SpriteBatch spriteBatch) {
            spriteBatch.End();
            spriteBatch.Begin_World(shader: false);
        }
    }
}