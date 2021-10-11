using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;

namespace AQMod.Effects.ShaderData
{
    public class LightSourceAsThirdColorVariableArmorShaderData : ArmorShaderData
    {
        private readonly Vector3 _thirdColor;

        public LightSourceAsThirdColorVariableArmorShaderData(Ref<Effect> shader, string passName, Vector3 thirdColor) : base(shader, passName)
        {
            _thirdColor = thirdColor;
        }

        public LightSourceAsThirdColorVariableArmorShaderData(Ref<Effect> shader, string passName, Color thirdColor) : base(shader, passName)
        {
            _thirdColor = thirdColor.ToVector3();
        }

        protected override void Apply()
        {
            Shader.Parameters["uLightSource"].SetValue(_thirdColor);
            base.Apply();
        }
    }
}