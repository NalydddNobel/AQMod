using Terraria.Graphics.Shaders;

namespace AQMod.Items.Vanities.Dyes
{
    public class BreakdownDye : DyeItem
    {
        public override string Pass => "ColorDistortPass";

        public override ArmorShaderData CreateShaderData()
        {
            return base.CreateShaderData().UseOpacity(1f);
        }
    }
}