using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Items.Misc.Dyes.Outline;
public class RainbowOutlineDye : DyeItemBase {
    public override int Rarity => ItemRarityID.Green;

    public override string Pass => "OutlinePass";

    public override void SetStaticDefaults() {
        base.SetStaticDefaults();
        ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<OutlineDye>();
    }

    public override ArmorShaderData CreateShaderData() {
        return new ArmorShaderDataDynamicColor(Effect, Pass, (e, d) => Main.DiscoColor);
    }
}