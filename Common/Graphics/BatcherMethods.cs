using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace AQMod.Common.Graphics
{
    internal static class BatcherMethods
    {
        public static SpriteSortMode Regular => SpriteSortMode.Deferred;
        public static SpriteSortMode Shader => SpriteSortMode.Immediate;

        public static class Background
        {
            public static void Begin(SpriteBatch spriteBatch)
            {
                Begin(spriteBatch, Regular);
            }
            public static void Begin(SpriteBatch spriteBatch, SpriteSortMode spriteSort)
            {
                spriteBatch.Begin(spriteSort, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, Main.instance.Rasterizer, null, Main.BackgroundViewMatrix.TransformationMatrix);
            }
        }

        public static class Dusts
        {
            public static void Begin(SpriteBatch spriteBatch)
            {
                Begin(spriteBatch, Regular);
            }
            public static void Begin(SpriteBatch spriteBatch, SpriteSortMode spriteSort)
            {
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);
            }
        }

        public static class GeneralEntities
        {
            public static void Begin(SpriteBatch spriteBatch)
            {
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.Transform);
            }
            public static void BeginShader(SpriteBatch spriteBatch)
            {
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, Main.instance.Rasterizer, null, Main.Transform);
            }
        }

        public static class Tiles
        {
            public static void Begin(SpriteBatch spriteBatch)
            {
                Begin(spriteBatch, Regular);
            }
            public static void Begin(SpriteBatch spriteBatch, SpriteSortMode spriteSort)
            {
                spriteBatch.Begin(spriteSort, null, null, null, null, null, Matrix.Identity);
            }
        }

        public static class UI
        {
            public static void Begin(SpriteBatch spriteBatch)
            {
                Begin(spriteBatch, Regular);
            }
            public static void Begin(SpriteBatch spriteBatch, SpriteSortMode spriteSort)
            {
                spriteBatch.Begin(spriteSort, null, null, null, null, null, Main.UIScaleMatrix);
            }
        }
    }
}