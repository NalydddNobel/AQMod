using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;

namespace Aequus.Graphics.ShaderData
{
    public class ArmorCustomTexture : ArmorShaderData
    {
        public Ref<Texture2D> _texture;

        public ArmorCustomTexture(Ref<Effect> shader, string passName, Ref<Texture2D> texture) : base(shader, passName)
        {
            _texture = texture;
        }

        public override void Apply()
        {
            var t = _texture.Value;
            Main.graphics.GraphicsDevice.Textures[1] = t;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.LinearWrap;
            Shader.Parameters["uImageSize1"].SetValue(new Vector2(t.Width, t.Height));
            base.Apply();
        }
    }
}