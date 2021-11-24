using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;

namespace AQMod.Effects.Dyes
{
    public class CustomTextureArmorShaderData : ArmorShaderData
    {
        private readonly Ref<Texture2D> _texture;

        public CustomTextureArmorShaderData(Ref<Effect> shader, string passName, Ref<Texture2D> texture) : base(shader, passName)
        {
            _texture = texture;
        }

        protected override void Apply()
        {
            var t = _texture.Value;
            Main.graphics.GraphicsDevice.Textures[1] = t;
            Shader.Parameters["uImageSize1"].SetValue(new Vector2(t.Width, t.Height));
            base.Apply();
        }
    }
}