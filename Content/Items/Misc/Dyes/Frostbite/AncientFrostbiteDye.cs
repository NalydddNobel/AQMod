using Aequus.Common.Items;
using Microsoft.Xna.Framework;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Items.Misc.Dyes.Frostbite;

public class AncientFrostbiteDye : DyeItemBase {
    public override string Pass => "AncientFrostbitePass";
    public override int Rarity => ItemCommons.Rarity.SpaceStormLoot - 1;

    public override void SetStaticDefaults() {
        base.SetStaticDefaults();
        ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<FrostbiteDye>();
    }

    public override ArmorShaderData CreateShaderData() {
        return new ArmorShaderData(Effect, Pass).UseColor(new Vector3(0.05f, 0.2f, 0.9f)).UseSecondaryColor(new Vector3(0.1f, 0.4f, 2f));
    }
}