using AQMod.Effects.Dyes;
using Microsoft.Xna.Framework;
using Terraria.Graphics.Shaders;

namespace AQMod.Items.Vanities.Dyes
{
    public class HellBeamDye : DyeItem
    {
        public override string Pass => "ShieldBeamsPass";
        public override ArmorShaderData CreateShaderData => new LightSourceAsThirdColorVariableArmorShaderData(Effect, Pass,
            new Vector3(0.3f, 0.2f, 0f)).UseColor(new Vector3(1f, 0.8f, 0.1f)).UseSecondaryColor(1.8f, 0.8f, 0.6f);
    }
}