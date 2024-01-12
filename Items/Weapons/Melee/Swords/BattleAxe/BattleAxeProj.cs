using Aequus.Common.Net.Sounds;
using Aequus.Common.Projectiles.Base;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Melee.Swords.BattleAxe {
    public class BattleAxeProj : HeldSlashingSwordProjectile {
        public override void SetDefaults() {
            base.SetDefaults();
            Projectile.width = 90;
            Projectile.height = 90;
            Projectile.extraUpdates = 6;
            swordWidth = 30;
            swordHeight = 60;
            gfxOutOffset = -18;
        }

        public override void AI() {
            base.AI();
            if (Main.player[Projectile.owner].itemAnimation <= 1) {
                Main.player[Projectile.owner].Aequus().itemCombo = (ushort)(combo == 0 ? 64 : 0);
            }
            if (!playedSound && AnimProgress > 0.4f) {
                playedSound = true;
                SoundEngine.PlaySound(SoundID.Item1.WithPitchOffset(-1f), Projectile.Center);
            }
        }

        public override Vector2 GetOffsetVector(float progress) {
            return BaseAngleVector.RotatedBy((progress * (MathHelper.Pi * 1.75f) - MathHelper.PiOver2 * 1.75f) * -swingDirection * (0.9f + 0.1f * Math.Min(Main.player[Projectile.owner].Aequus().itemUsage / 300f, 1f)));
        }

        public override bool? CanDamage() {
            return AnimProgress > 0.05f && AnimProgress < 0.5f ? null : false;
        }

        public override float SwingProgress(float progress) {
            return SwingProgressStariteSword(progress);
        }

        public override float GetScale(float progress) {
            float scale = base.GetScale(progress);
            if (progress > 0.1f && progress < 0.9f) {
                return scale + 0.3f * MathF.Pow(MathF.Sin((progress - 0.1f) / 0.9f * MathHelper.Pi), 2f);
            }
            return scale;
        }

        public override float GetVisualOuter(float progress, float swingProgress) {
            if (progress > 0.6f) {
                float p = 1f - (1f - progress) / 0.4f;
                Projectile.alpha = (int)(p * 255);
                return -8f * p;
            }
            return 0f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            base.OnHitNPC(target, hit, damageDone);
            freezeFrame = 4;
            if (Main.rand.NextBool(5)) {
                ModContent.GetInstance<BleedingDebuffSound>().Play(target.Center);
                target.AddBuff(ModContent.BuffType<BattleAxeBleeding>(), 240);
            }
        }

        public override bool PreDraw(ref Color lightColor) {
            var drawColor = Projectile.GetAlpha(lightColor) * Projectile.Opacity;
            GetSwordDrawInfo(out var texture, out var handPosition, out var frame, out var rotationOffset, out var origin, out var effects);

            float colorIntensity = drawColor.ToVector3().Length() * Projectile.Opacity * 0.2f;
            Color swishColor = new(colorIntensity, colorIntensity, colorIntensity, 0f);

            DrawSwordAfterImages(texture, handPosition, frame, swishColor * 0.1f, rotationOffset, origin, effects);
            DrawSword(texture, handPosition, frame, drawColor, rotationOffset, origin, effects);
            DrawDebug();
            return false;
        }
    }
}