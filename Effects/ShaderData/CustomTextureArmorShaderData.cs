using AQMod.Assets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;

namespace AQMod.Effects.ShaderData
{
    public class CustomTextureArmorShaderData : ArmorShaderData
    {
        private readonly TextureAsset _texture;

        public CustomTextureArmorShaderData(Ref<Effect> shader, string passName, TextureAsset texture) : base(shader, passName)
        {
            _texture = texture;
        }

        protected override void Apply()
        {
            var t = _texture.GetValue();
            Main.graphics.GraphicsDevice.Textures[1] = t;
            Shader.Parameters["uImageSize1"].SetValue(new Vector2(t.Width, t.Height));
            base.Apply();
        }
    }
}