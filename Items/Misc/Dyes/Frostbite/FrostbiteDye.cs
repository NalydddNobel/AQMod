using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Misc.Dyes.Frostbite;

public class FrostbiteDye : DyeItemBase {
    public override string Pass => "FrostbitePass";
    public override int Rarity => ItemRarityID.Orange;

    public override void SetStaticDefaults() {
        base.SetStaticDefaults();
        ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<AncientFrostbiteDye>();
    }

    public override ArmorShaderData CreateShaderData() {
        return base.CreateShaderData().UseImage(AequusTextures.FrostbiteDyeEffect);
    }
}