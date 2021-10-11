using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace AQMod.Effects.Batchers
{
    public class DustBatcher : Batcher
    {
        public DustBatcher(SpriteBatch spriteBatch) : base(spriteBatch)
        {
        }

        public override void StartBatch()
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);
        }

        public override void StartShaderBatch()
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
        }
    }
}