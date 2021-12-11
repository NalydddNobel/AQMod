using Terraria.Graphics.Shaders;

namespace AQMod.Items.Vanities.Dyes
{
    public class DiscoDye : DyeItem
    {
        public override string Pass => "RainbowPass";

        public override ArmorShaderData CreateShaderData()
        {
            return base.CreateShaderData().UseOpacity(1f);
        }
    }
}