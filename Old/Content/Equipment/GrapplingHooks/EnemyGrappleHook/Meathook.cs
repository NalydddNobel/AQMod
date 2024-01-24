using Aequus.Old.Core.Assets;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.Localization;

namespace Aequus.Old.Content.Equipment.GrapplingHooks.EnemyGrappleHook;

public class Meathook : ModItem {
    public const string IMMUNE_TIMER = "Meathook Immunity";
    public static float DamageBonus { get; set; } = 0.1f;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ExtendLanguage.Percent(DamageBonus));

    public override void SetDefaults() {
        Item.CloneDefaults(ItemID.DualHook);
        Item.damage = 30;
        Item.knockBack = 0f;
        Item.shoot = ModContent.ProjectileType<MeathookProj>();
        Item.value = Item.buyPrice(gold: 10);
        Item.shootSpeed /= 2f;
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips) {
        tooltips.RemoveCritChance();
        tooltips.RemoveKnockback();
    }

    public override bool WeaponPrefix() {
        return true;
    }

    public static void CheckMeathookDamage(NPC target, ref NPC.HitModifiers modifiers) {
        if (target.HasBuff<MeathookDebuff>()) {
            modifiers.FinalDamage += DamageBonus;
            modifiers.Knockback *= 0.1f;
        }
    }

    public static void CheckMeathookSound(NPC target) {
        if (target.HasBuff<MeathookDebuff>()) {
            SoundEngine.PlaySound(OldAequusSounds.Meathook, target.Center);
        }
    }
}
