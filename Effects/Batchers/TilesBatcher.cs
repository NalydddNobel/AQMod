using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace AQMod.Effects.SpriteBatchModifers
{
    public class TilesBatcher : Batcher
    {
        public TilesBatcher(SpriteBatch spriteBatch) : base(spriteBatch)
        {
        }

        public override void StartBatch()
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Matrix.Identity);
        }

        public override void StartShaderBatch()
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, Matrix.Identity);
        }
    }
}