using Aequus.Common.Items;
using Aequus.Core.Autoloading;
using Microsoft.Xna.Framework;

namespace Aequus.Content.Bosses.BossMasks;

[AutoloadEquip(EquipType.Head)]
[AutoloadGlowMask("_Glow", "_Head_Glow")]
public class RedSpriteMask : ModItem {
    public override void SetStaticDefaults() {
        ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true;
    }

    public override void SetDefaults() {
        Item.DefaultToHeadgear(16, 16, Item.headSlot);
        Item.rare = ItemCommons.Rarity.BossMasks;
        Item.vanity = true;
        Item.GetGlobalItem<GravityGlobalItem>().itemGravityCheck = 255;
    }

    public override void DrawArmorColor(Player drawPlayer, float shadow, ref Color color, ref int glowMask, ref Color glowMaskColor) {
        glowMask = GlowMasksLoader.GetId(AequusTextures.RedSpriteMask_Head_Glow.Path);
        glowMaskColor = (Color.White with { A = 0 }) * (1f - shadow);
    }
}