using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Items.Misc.Dyes.Outline;

public class OutlineDye : DyeItemBase {
    public override string Pass => "OutlinePass";

    public override void SetStaticDefaults() {
        base.SetStaticDefaults();
        ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<RainbowOutlineDye>();
    }

    public override ArmorShaderData CreateShaderData() {
        return base.CreateShaderData().UseColor(1f, 1f, 1f);
    }
}