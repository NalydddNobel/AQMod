using Aequus.Common;
using Aequus.Common.Items;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Equipment.Vanity.Masks;

[AutoloadEquip(EquipType.Head)]
[AutoloadGlowMask("_Glow", "_Head_Glow")]
public class RedSpriteMask : ModItem {
    public override void SetStaticDefaults() {
        ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true;
    }

    public override void SetDefaults() {
        Item.DefaultToHeadgear(16, 16, Item.headSlot);
        Item.rare = ItemDefaults.RarityBossMasks;
        Item.vanity = true;
        Item.Aequus().itemGravityCheck = 255;
    }

    public override void DrawArmorColor(Player drawPlayer, float shadow, ref Color color, ref int glowMask, ref Color glowMaskColor) {
        glowMask = GlowMasksHandler.GetID(AequusTextures.RedSpriteMask_Head_Glow.Path);
        glowMaskColor = (Color.White with { A = 0 }) * (1f - shadow);
    }
}