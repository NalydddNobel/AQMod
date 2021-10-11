using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace AQMod.Effects.Batchers
{
    public class UIBatcher : Batcher
    {
        public UIBatcher(SpriteBatch spriteBatch) : base(spriteBatch)
        {
        }

        public override void StartBatch()
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Main.UIScaleMatrix);
        }

        public override void StartShaderBatch()
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, Main.UIScaleMatrix);
        }
    }
}