using AQMod.Effects.Dyes;
using Terraria;
using Terraria.Graphics.Shaders;

namespace AQMod.Items.Vanities.Dyes
{
    public class RainbowOutlineDye : DyeItem
    {
        public override string Pass => "OutlineColorPass";

        public override ArmorShaderData CreateShaderData()
        {
            return new DynamicColorArmorShaderData(Effect, Pass, () => Main.DiscoColor.ToVector3());
        }
    }
}