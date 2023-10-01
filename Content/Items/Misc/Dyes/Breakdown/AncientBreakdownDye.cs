using Terraria.Graphics.Shaders;

namespace Aequus.Content.Items.Misc.Dyes.Breakdown;

public class AncientBreakdownDye : DyeItemBase {
    public override string Pass => "BreakdownPass";

    public override ArmorShaderData CreateShaderData() {
        return new ArmorShaderData(Effect, Pass).UseOpacity(1f);
    }
}