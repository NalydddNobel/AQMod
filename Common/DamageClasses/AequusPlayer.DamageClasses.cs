using Aequus.Common.DamageClasses;
using Aequus.Common.DataSets;
using Terraria;
using Terraria.Audio;

namespace Aequus;

public partial class AequusPlayer {
    private void ModifyHit_SoulDamage(NPC target, ref NPC.HitModifiers modifiers) {
        if (modifiers.DamageType is ISoulDamageClass) {
            modifiers.HideCombatText();
            modifiers.Knockback *= 0f;
            modifiers.DefenseEffectiveness *= 0f;
        }
    }

    private void OnHit_SoulDamage(NPC target, NPC.HitInfo hit, int damageDone) {
        if (hit.DamageType is ISoulDamageClass) {
            if (target.life < target.lifeMax) {
                target.life += damageDone - 1;
            }
            int soulDamage = hit.Damage;
            CombatText.NewText(target.Hitbox, CombatText.DamagedHostile.HueSet(0.55f), soulDamage);

            if (target.TryGetAequus(out var aequusNPC)) {
                //aequusNPC.soulHealthTotal += soulDamage;

                //if (aequusNPC.soulHealthTotal > target.lifeMax) {
                //    if (NPCSets.Unfriendable.Contains(target.type)) {
                //        target.Kill(); // Kill on this client, this should also add a player interaction if needed.
                //        return;
                //    }

                //    target.life = target.lifeMax;
                //    target.friendly = true;
                //    target.dontTakeDamage = true;
                //    aequusNPC.zombieInfo.IsZombie = true;
                //    aequusNPC.zombieInfo.PlayerOwner = Player.whoAmI;
                //    aequusNPC.zombieInfo.SetDamage = hit.SourceDamage * 2;

                //    SoundEngine.PlaySound(AequusSounds.ghostification, target.Center);
                //}
            }
        }
    }
}