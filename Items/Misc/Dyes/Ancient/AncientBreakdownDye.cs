using Terraria.Graphics.Shaders;

namespace Aequus.Items.Misc.Dyes.Ancient {
    public class AncientBreakdownDye : DyeItemBase
    {
        public override string Pass => "ColorDistortPass";

        public override ArmorShaderData CreateShaderData()
        {
            return new ArmorShaderData(Effect, Pass).UseOpacity(1f);
        }
    }
}