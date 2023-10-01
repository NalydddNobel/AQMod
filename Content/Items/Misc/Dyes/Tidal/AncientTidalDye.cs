using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Items.Misc.Dyes.Tidal;

public class AncientTidalDye : DyeItemBase {
    public override string Pass => "AquaticShaderPass";

    public override void SetStaticDefaults() {
        base.SetStaticDefaults();
        ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<TidalDye>();
    }

    public override ArmorShaderData CreateShaderData() {
        return base.CreateShaderData().UseImage(AequusTextures.TidalDyeEffect).UseOpacity(1f);
    }
}