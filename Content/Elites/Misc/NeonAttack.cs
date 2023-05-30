using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Elites.Misc {
    public class NeonAttack : ModProjectile {
        private bool _didEffects;

        public override void SetStaticDefaults() {
            this.SetTrail(35);
            PushableEntities.AddProj(Type);
        }

        public override void SetDefaults() {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.timeLeft = 360;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 6;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 16 * 6;

            _didEffects = false;
        }

        public override Color? GetAlpha(Color lightColor) {
            return new Color(255, 255, 255, 200);
        }

        public override void AI() {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (Main.rand.NextBool(10)) {
                var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.PinkTorch, Scale: 1.5f * Projectile.scale * Projectile.Opacity);
                d.noGravity = true;
                d.fadeIn = d.scale + Main.rand.NextFloat(0.1f, 0.5f);
            }

            if (Projectile.numUpdates == -1) {
                Projectile.ai[0]++;
            }

            if (Projectile.ai[0] > 12f) {
                Projectile.alpha += 2;
                Projectile.scale -= 0.004f;
                if (Projectile.alpha > 255) {
                    Projectile.Kill();
                }
            }

            if (!_didEffects) {
                _didEffects = true;
                if (Main.netMode != NetmodeID.Server) {
                    var v = Vector2.Normalize(Projectile.velocity);
                    for (int i = 0; i < 20; i++) {
                        var d = Dust.NewDustDirect(Projectile.Center - new Vector2(12f), 24, 24, DustID.CrystalPulse2);
                        d.velocity = v.RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f)) * Main.rand.NextFloat(6f);
                        d.noGravity = true;
                        d.fadeIn = d.scale + Main.rand.NextFloat(1f);
                    }
                    SoundEngine.PlaySound(AequusSounds.neonShoot, Projectile.Center);
                }
            }
        }

        public override bool PreDraw(ref Color lightColor) {
            Projectile.GetDrawInfo(out var texture, out var offset, out var frame, out var origin, out int trailLength);

            for (int i = 0; i < trailLength; i++) {
                float progress = Helper.CalcProgress(trailLength, i);
                Main.EntitySpriteDraw(texture, Projectile.oldPos[i] + offset - Main.screenPosition, null, new Color(128, 80, 128, 100) * Projectile.Opacity * progress, Projectile.oldRot[i], origin, Projectile.scale * (0.66f + progress * 0.33f), SpriteEffects.FlipHorizontally, 0);
            }
            Main.EntitySpriteDraw(AequusTextures.Bloom0, Projectile.position + offset - Main.screenPosition, null, new Color(128, 80, 128, 0) * Projectile.Opacity * 0.8f, Projectile.rotation, AequusTextures.Bloom0.Size() / 2f, new Vector2(1.5f, 1f) * Projectile.scale * 0.66f, SpriteEffects.FlipHorizontally, 0);

            Main.EntitySpriteDraw(texture, Projectile.position + offset - Main.screenPosition, null, Projectile.GetAlpha(lightColor) * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, SpriteEffects.FlipHorizontally, 0);
            return false;
        }
    }
}