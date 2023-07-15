using Aequus.Common.DamageClasses;
using Terraria;

namespace Aequus;

public partial class AequusPlayer {
    private void ModifyHit_SoulDamage(NPC target, ref NPC.HitModifiers modifiers) {
        if (modifiers.DamageType is ISoulDamageClass) {
            modifiers.HideCombatText();
            modifiers.Knockback *= 0f;
            modifiers.DefenseEffectiveness *= 0f;
        }
    }

    private void OnHit_SoulDamage(NPC target, NPC.HitInfo hit) {
        if (hit.DamageType is ISoulDamageClass) {
            int soulDamage = hit.Damage;
            CombatText.NewText(target.Hitbox, CombatText.DamagedHostile.HueSet(0.55f), soulDamage);
            target.Aequus().soulHealth += soulDamage;
        }
    }
}