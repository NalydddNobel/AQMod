using Terraria.Graphics.Shaders;

namespace Aequus.Items.Misc.Dyes
{
    public class CensorDye : DyeItemBase
    {
        public override string Pass => "CensorPass";

        public override ArmorShaderData CreateShaderData()
        {
            return base.CreateShaderData().UseOpacity(4f);
        }
    }
}