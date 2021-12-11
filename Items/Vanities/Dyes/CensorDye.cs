using Terraria.Graphics.Shaders;

namespace AQMod.Items.Vanities.Dyes
{
    public class CensorDye : DyeItem
    {
        public override string Pass => "CensorPass";

        public override ArmorShaderData CreateShaderData()
        {
            return base.CreateShaderData().UseOpacity(4f);
        }
    }
}