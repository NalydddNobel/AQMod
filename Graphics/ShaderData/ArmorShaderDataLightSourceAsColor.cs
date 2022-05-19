using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;

namespace Aequus.Graphics.ShaderData
{
    public class ArmorShaderDataLightSourceAsColor : ArmorShaderData
    {
        private readonly Vector3 _thirdColor;

        public ArmorShaderDataLightSourceAsColor(Ref<Effect> shader, string passName, Vector3 thirdColor) : base(shader, passName)
        {
            _thirdColor = thirdColor;
        }

        public ArmorShaderDataLightSourceAsColor(Ref<Effect> shader, string passName, Color thirdColor) : base(shader, passName)
        {
            _thirdColor = thirdColor.ToVector3();
        }

        public override void Apply()
        {
            Shader.Parameters["uLightSource"].SetValue(_thirdColor);
            base.Apply();
        }
    }
}