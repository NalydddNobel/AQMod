using Microsoft.Xna.Framework.Graphics;

namespace AQMod.Effects.Batchers
{
    public abstract class Batcher
    {
        protected SpriteBatch spriteBatch;

        public Batcher(SpriteBatch spriteBatch)
        {
            this.spriteBatch = spriteBatch;
        }

        public abstract void StartBatch();
        public abstract void StartShaderBatch();
    }
}