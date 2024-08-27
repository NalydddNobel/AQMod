using Aequus;
using Aequus.Buffs.Debuffs;
using Aequus.Common.Buffs;
using Aequus.Common.DataSets;
using Aequus.Common.Net.Sounds;
using Aequus.Content;
using Aequus.Particles.Dusts;
using Aequus.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Melee.Swords.SuperStarSword {
    public class SuperStarSwordSlash : ModProjectile {
        private bool _didEffects;

        public override void SetStaticDefaults() {
            this.SetTrail(18);
            LegacyPushableEntities.AddProj(Type);
            ProjectileSets.DealsHeatDamage.Add(Type);
        }

        public override void SetDefaults() {
            Projectile.width = 102;
            Projectile.height = 102;
            Projectile.timeLeft = 120;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.extraUpdates = 3;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 16 * 6;

            _didEffects = false;
        }

        public override Color? GetAlpha(Color lightColor) {
            return new Color(255, 250, 255, 0);
        }

        public override void AI() {
            var r = Utils.CenteredRectangle(Projectile.Center, new Vector2(Projectile.width / 4f, Projectile.height / 4f));
            if (Collision.SolidCollision(r.TopLeft(), r.Width, r.Height)) {
                Projectile.velocity *= 0.95f;
                Projectile.damage -= 1;
                if (Projectile.timeLeft > 32)
                    Projectile.timeLeft--;
                if (Projectile.damage < 0) {
                    if (Projectile.velocity.Length() > 2f)
                        Projectile.velocity *= 0.9f;
                    Projectile.damage = 0;
                }
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (Projectile.timeLeft < 32) {
                Projectile.alpha += 8;
                Projectile.scale -= 0.008f;
                if (Projectile.alpha > 255) {
                    Projectile.Kill();
                }
            }
            else {
                Projectile.velocity *= 0.975f;
                if (Main.rand.NextBool((int)Math.Max(12f - Projectile.velocity.Length(), 2f))) {
                    var d = Dust.NewDustPerfect(Projectile.Center + Vector2.Normalize(Projectile.velocity.RotatedBy(MathHelper.PiOver2)) * Main.rand.NextFloat(-50f, 50f) * Projectile.scale,
                        ModContent.DustType<MonoDust>(), -Projectile.velocity * 0.2f, 0, Main.rand.Next(SuperStarSwordProj.DustColors), Main.rand.NextFloat(0.9f, 1.45f));
                    d.noGravity = true;
                }
            }

            if (!_didEffects) {
                _didEffects = true;
                if (Main.netMode != NetmodeID.Server) {
                    SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot.WithPitch(0.33f), Projectile.Center);
                }
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
            float _ = float.NaN;
            var normal = new Vector2(1f, 0f).RotatedBy(Projectile.rotation);
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(),
                Projectile.Center + normal * -46f, Projectile.Center + normal * 46f, 32f * Projectile.scale, ref _);
        }

        public void OnHit(Entity target) {
            if (Main.rand.NextBool(4)) {
                AequusBuff.ApplyBuff<BlueFire>(target, 240, out bool canPlaySound);
                if (canPlaySound) {
                    ModContent.GetInstance<BlueFireDebuffSound>().Play(target.Center);
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            Projectile.damage = (int)(Projectile.damage * 0.9f);
            OnHit(target);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info) {
            OnHit(target);
        }

        public override bool PreDraw(ref Color lightColor) {
            Projectile.GetDrawInfo(out var texture, out var offset, out var frame, out var origin, out int trailLength);
            var color = Main.tenthAnniversaryWorld ? Color.HotPink with { A = 0 } * 0.45f : new Color(8, 50, 128, 30);
            for (int i = 0; i < trailLength; i++) {
                float progress = Helper.CalcProgress(trailLength, i);
                Main.EntitySpriteDraw(texture, Projectile.oldPos[i] + offset - Main.screenPosition, null, color * Projectile.Opacity * progress, Projectile.oldRot[i], origin, Projectile.scale * (0.5f + progress * 0.5f) * 0.9f, SpriteEffects.FlipHorizontally, 0);
            }
            Main.EntitySpriteDraw(AequusTextures.Bloom0, Projectile.position + offset - Main.screenPosition, null, color * Projectile.Opacity * 0.8f, Projectile.rotation, AequusTextures.Bloom0.Size() / 2f, new Vector2(1.5f, 1f) * Projectile.scale, SpriteEffects.FlipHorizontally, 0);
            Main.EntitySpriteDraw(texture, Projectile.position + offset - Main.screenPosition, null, Projectile.GetAlpha(lightColor) * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, SpriteEffects.FlipHorizontally, 0);
            return false;
        }
    }
}