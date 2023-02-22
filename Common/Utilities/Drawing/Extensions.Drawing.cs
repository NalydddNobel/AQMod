using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Aequus
{
    internal static partial class Extensions
    {
        private const SpriteSortMode SortMode_Default = SpriteSortMode.Deferred;
        private const SpriteSortMode SortMode_Shader = SpriteSortMode.Immediate;

        public static void Begin_Dusts(this SpriteBatch spriteBatch, bool immediate = false)
        {
            spriteBatch.Begin(!immediate ? SortMode_Default : SortMode_Shader, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);
        }

        public static void Begin_World(this SpriteBatch spriteBatch, bool shader = false)
        {
            if (!shader)
            {
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
            }
            else
            {
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, Main.Rasterizer, null, Main.Transform);
            }
        }
        
        public static void Begin_Tiles(this SpriteBatch spriteBatch, bool shader = false)
        {
            spriteBatch.Begin(!shader ? SortMode_Default : SortMode_Shader, null, null, null, null, null, Matrix.Identity);
        }

        public static void Begin_UI(this SpriteBatch spriteBatch, bool immediate = false, bool useScissorRectangle = false, Matrix? matrix = null)
        {
            RasterizerState rasterizer = null;
            if (useScissorRectangle)
            {
                rasterizer = new RasterizerState
                {
                    CullMode = CullMode.None,
                    ScissorTestEnable = true
                };
            }
            spriteBatch.Begin(immediate ? SortMode_Default : SortMode_Shader, null, null, null, rasterizer, null, matrix ?? Main.UIScaleMatrix);
        }

        public static class UI
        {
            public static void Begin(SpriteBatch spriteBatch, SpriteSortMode spriteSort, bool useScissorRectangle = false)
            {
            }
            public static void BeginWMatrix(SpriteBatch spriteBatch, bool useScissorRectangle = false, Matrix? matrix = null)
            {
                BeginWMatrix(spriteBatch, SortMode_Default, useScissorRectangle, matrix);
            }
            public static void BeginWMatrix(SpriteBatch spriteBatch, SpriteSortMode spriteSort, bool useScissorRectangle = false, Matrix? matrix = null)
            {
                RasterizerState rasterizer = null;
                if (useScissorRectangle)
                {
                    rasterizer = new RasterizerState
                    {
                        CullMode = CullMode.None,
                        ScissorTestEnable = true
                    };
                }
                spriteBatch.Begin(spriteSort, null, null, null, rasterizer, null, matrix ?? Main.UIScaleMatrix);
            }
        }
    }
}