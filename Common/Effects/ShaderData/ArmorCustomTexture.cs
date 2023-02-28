using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics.Shaders;

namespace Aequus.Common.Effects.ShaderData
{
    public class ArmorCustomTexture : ArmorShaderData
    {
        public Asset<Texture2D> _texture;

        public ArmorCustomTexture(Ref<Effect> shader, string passName, Asset<Texture2D> texture) : base(shader, passName)
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