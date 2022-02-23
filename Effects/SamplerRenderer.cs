using AQMod.Assets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;

namespace AQMod.Effects
{
    public static class SamplerRenderer
    {
        public static void Light(Vector2 position, float scale)
        {
            Light(position, new Vector2(scale, scale));
        }

        public static void Light(Vector2 position, Vector2 scale)
        {
            Light(position, scale, new Color(255, 255, 255, 255));
        }

        public static void Light(Vector2 position, float scale, Color color)
        {
            Light(position, new Vector2(scale, scale), color);
        }

        public static void Light(Vector2 position, Vector2 scale, Color color)
        {
            Light(position, 0f, scale, color);
        }

        public static void Light(Vector2 position, float rotation, Vector2 scale, Color color)
        {
            DrawSampler(LegacyEffectCache.s_Spotlight, position, rotation, scale, color);
        }

        public static void DrawSampler(string name, Vector2 position, float rotation, Vector2 scale, Color color)
        {
            DrawSampler(GameShaders.Misc[name], position, rotation, scale, color);
        }

        public static void DrawSampler(MiscShaderData effect, Vector2 position, float rotation, Vector2 scale, Color color)
        {
            var sampler = Tex.Pixel.Texture.Value;
            var drawData = new DrawData(sampler, position, null, color, rotation, new Vector2(0.5f, 0.5f), scale, SpriteEffects.None, 0);
            effect.UseColor(color);
            effect.Apply(drawData);
            drawData.Draw(Main.spriteBatch);
        }
    }
}