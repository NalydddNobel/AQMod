using Aequus.Common.Particles;
using Aequus.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Misc.CrownOfBlood {
    public class NaniteExplosion : ModProjectile {
        public override string Texture => AequusTextures.Explosion1.Path;

        public override void SetStaticDefaults() {
            Main.projFrames[Type] = 7;
        }

        public override void SetDefaults() {
            Projectile.DefaultToExplosion(90, DamageClass.Generic, 20);
            Projectile.idStaticNPCHitCooldown /= 4;
        }

        public override Color? GetAlpha(Color lightColor) {
            return new Color(255, 0, 0, 100);
        }

        public override void AI() {
            if (Projectile.frame == 0 && Main.netMode != NetmodeID.Server) {
                for (int i = 0; i < 10; i++) {
                    var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch);
                    d.fadeIn = d.scale + 0.1f;
                    d.noGravity = true;
                }
                for (int i = 0; i < 5; i++) {
                    var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke);
                    d.fadeIn = d.scale + 0.1f;
                    d.noGravity = true;
                }
                for (int i = 0; i < 20; i++) {
                    var v = Main.rand.NextVector2Unit();
                    ParticleSystem.New<MonoBloomParticle>(ParticleLayer.BehindPlayers).Setup(Projectile.Center + v * Main.rand.NextFloat(16f), v * Main.rand.NextFloat(3f, 12f),
                        new Color(255, 50, 30, 100), new Color(10, 0, 0, 0), Main.rand.NextFloat(1f, 2f), 0.3f);
                }
            }
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 2) {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= Main.projFrames[Type]) {
                    Projectile.hide = true;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor) {
            Projectile.GetDrawInfo(out var texture, out var offset, out var frame, out var origin, out int _);
            Main.spriteBatch.Draw(texture, Projectile.position + offset - Main.screenPosition, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}