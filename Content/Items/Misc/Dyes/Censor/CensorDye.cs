using Terraria.Graphics.Shaders;

namespace Aequus.Content.Items.Misc.Dyes.Censor;

public class CensorDye : DyeItemBase {
    public override string Pass => "CensorPass";

    public override ArmorShaderData CreateShaderData() {
        return base.CreateShaderData().UseOpacity(4f);
    }
}