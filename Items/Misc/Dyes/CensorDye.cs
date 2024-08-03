using Terraria.Graphics.Shaders;

namespace Aequus.Items.Misc.Dyes;
[LegacyName("AncientHellBeamDye")]
public class CensorDye : DyeItemBase {
    public override string Pass => "CensorPass";

    public override ArmorShaderData CreateShaderData() {
        return base.CreateShaderData().UseOpacity(4f);
    }
}