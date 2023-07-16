using Aequus;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Necromancy.Sceptres {
    public abstract class SceptreAttackProjBase : ModProjectile {
        protected abstract int DustType { get; }
        protected abstract int HitSparkleProjectile { get; }

        public override void SetDefaults() {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.DamageType = Aequus.NecromancyMagicClass;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.alpha = 255;
            Projectile.timeLeft = 120;
            Projectile.extraUpdates = 120;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 50;
        }

        public override Color? GetAlpha(Color lightColor) {
            return Color.White with { A = 0 } * (Projectile.alpha > 0 ? 0f : 1f);
        }

        public override void AI() {
            if ((int)Projectile.ai[0] == 1) {
                Projectile.timeLeft = 2;
                Projectile.extraUpdates = 3;
                Projectile.tileCollide = false;
                if (Projectile.localAI[0] == 0) {
                    Projectile.localAI[0] = 1f;
                }

                var center = Projectile.Center;
                float wave = Helper.Wave(Main.GameUpdateCount * Projectile.MaxUpdates + Projectile.numUpdates, 1f, 10f);
                var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustType, Projectile.velocity.X, Projectile.velocity.Y, Scale: Main.rand.NextFloat(1.25f, 0.75f));
                d.velocity *= 0.6f;

                var difference = Main.player[Projectile.owner].Center - Projectile.Center;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.Normalize(difference) * 4f, 0.12f);
                if (difference.Length() < 50f) {
                    Projectile.Kill();
                }
                return;
            }

            if (Projectile.alpha <= 0) {
                //var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<ZombieSceptreParticle>(), Projectile.velocity.X, Projectile.velocity.Y, Scale: Main.rand.NextFloat(1.25f, 0.75f));
                //d.velocity *= 0.6f;
                if (Projectile.ai[1] == 0f) {
                    Projectile.ai[1] = Projectile.velocity.X;
                    Projectile.ai[2] = Projectile.velocity.Y;
                }
                int target = Projectile.FindTargetWithLineOfSight(50f);
                if (target != -1) {
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(Main.npc[target]) * 4f, 0.1f);
                }
                else if (Projectile.timeLeft % 10 == 0) {
                    Projectile.netUpdate = true;
                    Projectile.velocity = new Vector2(Projectile.ai[1], Projectile.ai[2]).RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f));
                }
            }
            else {
                Projectile.alpha -= 10;
                if (Projectile.alpha < 0) {
                    Projectile.alpha = 0;
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            Projectile.ai[0] = 1f;
            Projectile.damage = 0;
            Projectile.velocity = -Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f));
            Projectile.netUpdate = true;
            Projectile.NewProjectile(Projectile.GetSource_OnHit(target), Projectile.Center, Vector2.Zero, HitSparkleProjectile, 0, 0f, Projectile.owner);
        }

        public override void Kill(int timeLeft) {
            if (Projectile.ai[0] <= 0f) {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, HitSparkleProjectile, 0, 0f, Projectile.owner);
            }
        }

        public override bool PreDraw(ref Color lightColor) {
            return Projectile.ai[0] > 0f;
        }
    }
}