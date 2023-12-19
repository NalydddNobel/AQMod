using Aequus.Common.Items;
using Microsoft.Xna.Framework;

namespace Aequus.Content.Bosses.BossMasks;

[AutoloadEquip(EquipType.Head)]
public class DustDevilMaskIce : ModItem {
    public override void SetDefaults() {
        Item.DefaultToHeadgear(16, 16, Item.headSlot);
        Item.rare = ItemCommons.Rarity.BossMasks;
        Item.vanity = true;
        Item.GetGlobalItem<GravityGlobalItem>().itemGravityCheck = 255;
    }

    public override Color? GetAlpha(Color lightColor) {
        return Color.Lerp(Color.White, lightColor, 0.5f);
    }

    public override void DrawArmorColor(Player drawPlayer, float shadow, ref Color color, ref int glowMask, ref Color glowMaskColor) {
        color = Color.Lerp(Color.White, color, 0.5f);
    }
}