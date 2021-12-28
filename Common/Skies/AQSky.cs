using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Utilities;

namespace AQMod.Common.Skies
{
    /// <summary>
    /// An abstract custom sky which contains many useful methods.
    /// </summary>
    public abstract class AQSky : CustomSky
    {
        protected const float RecommendedMinDepth = 0f;
        protected const float RecommendedMaxDepth = 10f;

        protected UnifiedRandom rand;

        public AQSky()
        {
            rand = new UnifiedRandom();
        }

        public bool SkyDepth(float maxDepth, float minDepth)
        {
            return maxDepth == float.MaxValue && minDepth != float.MaxValue;
        }

        protected interface IDepthModule
        {
            bool Active { get; }
            Vector2 Position { get; }
            float Depth { get; }
        }

        protected TDepth[] SortDepth<TDepth>(TDepth[] array) where TDepth : IDepthModule
        {
            return SortDepth(new List<TDepth>(array)).ToArray();
        }

        protected List<TDepth> SortDepth<TDepth>(List<TDepth> list) where TDepth : IDepthModule
        {
            list.Sort((depth, depth2) => depth.Depth.CompareTo(depth2.Depth));
            return list;
        }

        protected void GetDepthDrawingIndices<TDepth>(out int minI, out int maxI, float minDepth, float maxDepth, TDepth[] array) where TDepth : IDepthModule
        {
            GetDepthDrawingIndices(out minI, out maxI, minDepth, maxDepth, new List<TDepth>(array));
        }

        protected void GetDepthDrawingIndices<TDepth>(out int minI, out int maxI, float minDepth, float maxDepth, List<TDepth> list) where TDepth : IDepthModule
        {
            minI = -1;
            maxI = 0;
            for (int j = 0; j < list.Count; j++)
            {
                float depth = list[j].Depth;
                if (minI == -1 && depth < maxDepth)
                {
                    minI = j;
                }
                if (depth <= minDepth)
                {
                    break;
                }
                maxI = j;
            }
            if (minI == -1)
            {
                return;
            }
        }

        protected bool CanEvenSeeTheSkyAtAll()
        {
            if (Main.screenPosition.Y > Main.worldSurface * 16.0 || Main.gameMenu)
            {
                return false;
            }
            return true;
        }

        protected void RenderDepthObjects(IDepthModule[] modules, float maxDepth, float minDepth)
        {
            int num = -1;
            int num2 = 0;
            for (int j = 0; j < modules.Length; j++)
            {
                float depth = modules[j].Depth;
                if (num == -1 && depth < maxDepth)
                {
                    num = j;
                }
                if (depth <= minDepth)
                {
                    break;
                }
                num2 = j;
            }
            if (num == -1)
            {
                return;
            }
            Vector2 value = Main.screenPosition + new Vector2(Main.screenWidth >> 1, Main.screenHeight >> 1);
            Rectangle clippingFrame = new Rectangle(-1000, -1000, 4000, 4000);
            for (int k = num; k < num2; k++)
            {
                if (modules[k].Active)
                {
                    var value2 = new Vector2(1f / modules[k].Depth, 0.9f / modules[k].Depth);
                    var drawPosition = modules[k].Position;
                    drawPosition = (drawPosition - value) * value2 + value - Main.screenPosition;
                    drawPosition.X = (drawPosition.X + 500f) % 4000f;
                    if (drawPosition.X < 0f)
                    {
                        drawPosition.X += 4000f;
                    }
                    drawPosition.X -= 500f;
                    if (ShouldRenderModule(modules[k], k, clippingFrame, drawPosition))
                    {
                    }
                }
            }
        }

        protected virtual bool ShouldRenderModule(IDepthModule module, int index, Rectangle clippingFrame, Vector2 drawPosition)
        {
            return clippingFrame.Contains((int)drawPosition.X, (int)drawPosition.Y);
        }

        protected float getDepthColorMultiplier(IDepthModule module)
        {
            float depthColorMult = 1f;
            if (module.Depth > 3f)
            {
                depthColorMult = 0.6f;
            }
            else if (module.Depth > 2.5f)
            {
                depthColorMult = 0.7f;
            }
            else if (module.Depth > 2f)
            {
                depthColorMult = 0.8f;
            }
            else if (module.Depth > 1.5f)
            {
                depthColorMult = 0.9f;
            }
            depthColorMult *= 0.9f;
            return depthColorMult;
        }

        protected Color getBGColor(float colorMultiplier = 1f)
        {
            Color color = new Color(Main.bgColor.ToVector4() * 0.9f + new Vector4(0.1f)) * 0.8f; 
            return new Color((int)(color.R * colorMultiplier), (int)(color.G * colorMultiplier), (int)(color.B * colorMultiplier), (int)(color.A * colorMultiplier));
        }

        protected virtual void RenderDepthObject(IDepthModule module, int index, Vector2 drawPosition)
        {
            //Main.spriteBatch.Draw(modules[k].Texture, vector, modules[k].GetSourceRectangle(), color * _opacity, 0f, Vector2.Zero, value2.X * 2f, SpriteEffects.None, 0f);
        }
    }
}