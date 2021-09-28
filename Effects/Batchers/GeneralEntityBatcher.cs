using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace AQMod.Effects.SpriteBatchModifers
{
    public class GeneralEntityBatcher : Batcher
    {
        public GeneralEntityBatcher(SpriteBatch spriteBatch) : base(spriteBatch)
        {
        }

        public override void StartBatch()
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.Transform);
        }

        public override void StartShaderBatch()
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, Main.instance.Rasterizer, null, Main.Transform);
        }
    }
}