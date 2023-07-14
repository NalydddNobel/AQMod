using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Necromancy.Sceptres.Zombie {
    public class ZombieSceptreProj : ModProjectile {
        public override void SetDefaults() {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.DamageType = Aequus.NecromancyMagicClass;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.alpha = 255;
            Projectile.extraUpdates = 5;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override Color? GetAlpha(Color lightColor) {
            return Color.White with { A = 0 };
        }

        public override void AI() {
            if ((int)Projectile.ai[0] == 1) {
                if (Projectile.localAI[0] == 0) {
                    Projectile.localAI[0] = 1f;
                }

                var center = Projectile.Center;
                float wave = Helper.Wave(Main.GameUpdateCount * Projectile.MaxUpdates + Projectile.numUpdates, 1f, 10f);
                var d = Dust.NewDustPerfect(center + Main.rand.NextVector2Square(-wave, wave), ModContent.DustType<ZombieSceptreParticle>(), Scale: Main.rand.NextFloat(1.8f, 1f));
                d.velocity = Projectile.velocity * 0.1f;

                var difference = Main.player[Projectile.owner].Center - Projectile.Center;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.Normalize(difference) * 6f, 0.06f);
                if (difference.Length() < 10f) {
                    Projectile.Kill();
                }
                return;
            }

            if (Main.rand.NextBool(Projectile.MaxUpdates)) {
                var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<ZombieSceptreParticle>(), -Projectile.velocity.X, -Projectile.velocity.Y, Scale: Main.rand.NextFloat(1.25f, 0.75f));
                d.velocity *= 0.2f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            Projectile.ai[0] = 1f;
            Projectile.damage = 0;
            Projectile.velocity = -Projectile.velocity;
            Projectile.netUpdate = true;
            Projectile.NewProjectile(Projectile.GetSource_OnHit(target), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<ZombieSceptreProjOnHit>(), 0, 0f, Projectile.owner);
        }

        public override bool PreDraw(ref Color lightColor) {
            return Projectile.ai[0] <= 0f;
        }
    }
}