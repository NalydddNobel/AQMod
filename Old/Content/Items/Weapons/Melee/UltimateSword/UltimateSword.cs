using Aequu2.Core;
using tModLoaderExtended.GlowMasks;

namespace Aequu2.Old.Content.Items.Weapons.Melee.UltimateSword;

[AutoloadGlowMask]
public class UltimateSword : ModItem {
    public override void SetDefaults() {
        Item.LazyCustomSwordDefaults<UltimateSwordProj>(24);
        Item.SetWeaponValues(50, 4.5f, 11);
        Item.width = 30;
        Item.height = 30;
        Item.scale = 1f;
        Item.rare = Commons.Rare.BossOmegaStarite;
        Item.value = Commons.Cost.BossOmegaStarite;
        Item.autoReuse = true;
    }

    public override bool? UseItem(Player player) {
        return null;
    }

    public override bool MeleePrefix() {
        return true;
    }

    public override void HoldItem(Player player) {
        player.AddBuff(ModContent.BuffType<UltimateSwordBuff>(), 1, quiet: true);
    }
}