using System;
using Terraria.DataStructures;

namespace Aequus.Content.Items.Armor.Castaway;

public class CastawayPlayer : ModPlayer {
    public bool setbonus;

    public float kbResist;

    public int brokenDefenseMax;
    public int brokenDefense;
    public int defenseRegeneration;

    private bool breakArmorAnim;

    public override void ResetEffects() {
        setbonus = false;
        kbResist = 1f;
        brokenDefenseMax = 0;
    }

    public override void ModifyHurt(ref Player.HurtModifiers modifiers) {
        modifiers.Knockback *= Math.Max(kbResist, 0f);
    }

    public override void OnHurt(Player.HurtInfo info) {
        int defenseDamageDivisor = Math.Max((int)(10 * Player.DefenseEffectiveness.Value), 1);
        int defenseDamage = Math.Max(info.Damage / defenseDamageDivisor, 1);
        if (brokenDefense < brokenDefenseMax) {
            brokenDefense = Math.Min(brokenDefense + defenseDamage, brokenDefenseMax);
        }
        if (setbonus) {
            if (Main.myPlayer == Player.whoAmI) {
                int projectileAmount = Math.Min(defenseDamage, CastawayArmor.MaxBallsOnHit);
                for (int i = 0; i < projectileAmount; i++) {
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, (i * MathHelper.TwoPi / 3f + Main.rand.NextFloat(-1f, 1f)).ToRotationVector2() * Main.rand.NextFloat(2f, 4f), ModContent.ProjectileType<CastawayProjExplosiveMine>(), 15, 0.1f, Player.whoAmI);
                }
            }
        }

        if (brokenDefense > 0 && Main.netMode != NetmodeID.Server) {
            ModContent.GetInstance<DefenseDamageUI>().OnPlayerHurt(Player, this);
        }

        defenseRegeneration = Math.Min(defenseRegeneration, -CastawayArmor.DefenseRegenerationCooldown);
    }

    public override void PostUpdateEquips() {
        if (brokenDefense <= 0) {
            return;
        }

        Player.statDefense -= brokenDefense;

        defenseRegeneration++;
        if (defenseRegeneration >= CastawayArmor.DefenseRegenerationRate) {
            defenseRegeneration = 0;
            brokenDefense--;

            if (Main.netMode != NetmodeID.Server) {
                ModContent.GetInstance<DefenseDamageUI>().OnRegenTick(Player, this);
            }
        }
    }

    public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo) {
        if (brokenDefense <= 0 || brokenDefense <= brokenDefenseMax / 2) {
            breakArmorAnim = false;
            return;
        }

        if (drawInfo.drawPlayer.head == CastawayArmor.HeadTextureId) {
            drawInfo.drawPlayer.head = CastawayArmor.Head2TextureId;
            if (!breakArmorAnim) {
                ExtendGore.NewGore(AequusTextures.CastawayHelmetBreak, Player.GetSource_FromThis(), Player.Top, Player.velocity);
            }
        }
        if (drawInfo.drawPlayer.body == CastawayArmor.BodyTextureId) {
            drawInfo.drawPlayer.body = CastawayArmor.Body2TextureId;
            if (!breakArmorAnim) {
                for (int i = 0; i < 2; i++) {
                    ExtendGore.NewGore(AequusTextures.CastawayChestplateBreak, Player.GetSource_FromThis(), Player.Center + Main.rand.NextVector2Circular(Player.width / 3f, 8), Player.velocity);
                }
            }
        }
        breakArmorAnim = true;
    }
}