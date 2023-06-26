using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Melee.UltimateSword;

[AutoloadGlowMask]
public class UltimateSword : ModItem {
    public override void SetDefaults() {
        Item.DefaultToAequusSword<UltimateSwordProj>(24);
        Item.SetWeaponValues(50, 4.5f, 11);
        Item.width = 30;
        Item.height = 30;
        Item.scale = 1f;
        Item.rare = ItemDefaults.RarityOmegaStarite;
        Item.value = ItemDefaults.ValueOmegaStarite;
        Item.autoReuse = true;
    }

    public override bool? UseItem(Player player) {
        Item.FixSwing(player);
        return null;
    }

    public override bool MeleePrefix() {
        return true;
    }

    public override void HoldItem(Player player) {
        player.AddBuff(ModContent.BuffType<UltimateSwordBuff>(), 1, quiet: true);
    }
}