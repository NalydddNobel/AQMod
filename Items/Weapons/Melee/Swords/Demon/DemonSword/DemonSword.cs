using Aequus.Buffs.Debuffs;
using Aequus.Common;
using Aequus.Common.Items;

namespace Aequus.Items.Weapons.Melee.Swords.Demon.DemonSword;

[AutoloadGlowMask]
[WorkInProgress]
public class DemonSword : ModItem {
    public override void SetStaticDefaults() {
#if DEBUG
        Content.Events.DemonSiege.DemonSiegeSystem.RegisterSacrifice(new(ModContent.ItemType<Items.Weapons.Melee.Swords.Demon.HellsBoon.HellsBoon>(), Type, Content.Events.DemonSiege.EventTier.Hardmode) { DisableDecraft = true, });
        Content.Events.DemonSiege.DemonSiegeSystem.RegisterSacrifice(new(ModContent.ItemType<Items.Weapons.Melee.Swords.Demon.Cauterizer.Cauterizer>(), Type, Content.Events.DemonSiege.EventTier.Hardmode) { DisableDecraft = true, });
#endif
    }

    public override void SetDefaults() {
        Item.DefaultToAequusSword<DemonSwordProj>(60);
        Item.SetWeaponValues(70, 20f, 6);
        Item.width = 24;
        Item.height = 24;
        Item.scale = 1f;
        Item.autoReuse = true;
        Item.rare = ItemDefaults.RarityDemonSiegeTier2;
        Item.value = ItemDefaults.ValueDemonSiegeTier2;
    }

    public override Color? GetAlpha(Color lightColor) {
        return lightColor.MaxRGBA(100);
    }

    public override bool? UseItem(Player player) {
        Item.FixSwing(player);
        return null;
    }

    public override bool MeleePrefix() {
        return true;
    }

    public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone) {
        target.AddBuffs(240, 1, CorruptionHellfire.Debuffs);
        target.AddBuffs(240, 1, CrimsonHellfire.Debuffs);
    }
}
