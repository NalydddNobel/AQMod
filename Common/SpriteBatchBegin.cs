﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Aequus.Common
{
    internal static class SpriteBatchBegin
    {
        public static SpriteSortMode Regular => SpriteSortMode.Deferred;
        public static SpriteSortMode Shader => SpriteSortMode.Immediate;

        public static class Dusts
        {
            public static void Begin(SpriteBatch spriteBatch)
            {
                Begin(spriteBatch, Regular);
            }
            public static void Begin(SpriteBatch spriteBatch, SpriteSortMode spriteSort)
            {
                spriteBatch.Begin(spriteSort, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);
            }
        }

        public static class GeneralEntities
        {
            public static void Begin(SpriteBatch spriteBatch)
            {
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
            }
            public static void BeginShader(SpriteBatch spriteBatch)
            {
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, Main.Rasterizer, null, Main.Transform);
            }

            public static void Begin(SpriteBatch spriteBatch, Matrix matrix)
            {
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, matrix);
            }
            public static void BeginShader(SpriteBatch spriteBatch, Matrix matrix)
            {
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, Main.Rasterizer, null, matrix);
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
            public static void Begin(SpriteBatch spriteBatch, bool useScissorRectangle = false)
            {
                Begin(spriteBatch, Regular, useScissorRectangle);
            }
            public static void Begin(SpriteBatch spriteBatch, SpriteSortMode spriteSort, bool useScissorRectangle = false)
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
                spriteBatch.Begin(spriteSort, null, null, null, rasterizer, null, Main.UIScaleMatrix);
            }
            public static void BeginWMatrix(SpriteBatch spriteBatch, bool useScissorRectangle = false, Matrix? matrix = null)
            {
                BeginWMatrix(spriteBatch, Regular, useScissorRectangle, matrix);
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