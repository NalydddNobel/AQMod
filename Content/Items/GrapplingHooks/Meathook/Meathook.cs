using Aequus.Common.Items;
using Aequus.Common.Utilities;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.Localization;

namespace Aequus.Content.Items.GrapplingHooks.Meathook;

public class Meathook : ModItem {
    public const string Timer_Id = "Meathook Immunity";

    public static readonly float DamageBonus = 0.25f;
    public static readonly float SlowTargetMultiplier = 0.1f;
    public static readonly int ImmuneTime = 24;
    public static readonly int ImmuneCD = ImmuneTime * 2;

    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ALanguage.Percent(DamageBonus));

    public override void SetDefaults() {
        Item.CloneDefaults(ItemID.DualHook);
        Item.damage = 30;
        Item.knockBack = 0f;
        Item.DamageType = DamageClass.Generic;
        Item.shoot = ModContent.ProjectileType<MeathookProj>();
        Item.value = Item.buyPrice(gold: 10);
        Item.shootSpeed /= 2f;
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips) {
        tooltips.RemoveCritChance();
        tooltips.RemoveKnockback();
    }

    // Meathook can get prefixes.
    public override bool WeaponPrefix() {
        return true;
    }

    [Gen.AequusPlayer_ModifyHitNPC]
    public static void CheckMeathookDamage(NPC target, ref NPC.HitModifiers modifiers) {
        if (target.HasBuff<MeathookDebuff>()) {
            modifiers.FinalDamage += DamageBonus;
            modifiers.Knockback *= 0.1f;
        }
    }

    [Gen.AequusPlayer_OnHitNPC]
    public static void CheckMeathookSound(Player player, AequusPlayer aequus, NPC target, NPC.HitInfo hit) {
        if (target.HasBuff<MeathookDebuff>()) {
            SoundEngine.PlaySound(AequusSounds.MeathookDamageBonusProc, target.Center);
        }
    }
}