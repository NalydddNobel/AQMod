using Aequus.Graphics.ShaderData;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Aequus.Graphics
{
    public sealed class ModEffects : ILoadable
    {
        public static StaticMiscShaderInfo VerticalGradient { get; private set; }

        void ILoadable.Load(Mod mod)
        {
            if (!Main.dedServ)
            {
                VerticalGradient = new StaticMiscShaderInfo("MiscEffects", "Aequus:VerticalGradient", "TextureScrollingPass", true);
            }
        }

        void ILoadable.Unload()
        {
            VerticalGradient = null;
        }

        public static void DrawShader(MiscShaderData effect, SpriteBatch spriteBatch, Vector2 drawPosition, Color color = default(Color), float rotation = 0f, Vector2? scale = null)
        {
            var sampler = Images.Pixel.Value;
            var drawData = new DrawData(sampler, drawPosition, null, color, rotation, new Vector2(0.5f, 0.5f), scale ?? Vector2.One, SpriteEffects.None, 0);
            effect.UseColor(color);
            effect.Apply(drawData);
            drawData.Draw(spriteBatch);
        }
        public static void DrawShader(MiscShaderData effect, SpriteBatch spriteBatch, Vector2 drawPosition, Color color = default(Color), float rotation = 0f, float scale = 1f)
        {
            DrawShader(effect, spriteBatch, drawPosition, color, rotation, new Vector2(scale, scale));
        }
    }
}