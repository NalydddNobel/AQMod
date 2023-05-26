using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.CrownOfBlood.Projectiles {
    public class HivePackMinion : ModProjectile {
        public override void SetStaticDefaults() {
            Main.projFrames[Type] = 4;
        }

        public override void SetDefaults() {
            Projectile.width = 36;
            Projectile.height = 36;
            Projectile.timeLeft = 600;
            Projectile.friendly = true;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 20;
        }

        public override bool? CanCutTiles() {
            return false;
        }

        public override bool? CanDamage() {
            return Projectile.ai[0] > 0f;
        }

        public override bool OnTileCollide(Vector2 oldVelocity) {
            if (Projectile.velocity.X != oldVelocity.X) {
                Projectile.velocity.X = -oldVelocity.X * 0.9f;
            }
            if (Projectile.velocity.Y != oldVelocity.Y) {
                Projectile.velocity.Y = -oldVelocity.Y * 0.9f;
            }
            return false;
        }

        public override void AI() {
            NPC target = Projectile.FindTargetWithinRange(360f);
            if (target == null) {
                int targetIndex = Projectile.FindTargetWithLineOfSight(1000f);
                if (targetIndex != -1) {
                    target = Main.npc[targetIndex];
                }
            }

            Projectile.rotation = Projectile.velocity.X * 0.1f;
            Projectile.spriteDirection = -Math.Sign(Projectile.velocity.X);
            Projectile.LoopingFrame(3);
            Projectile.CollideWithOthers(speed: 0.3f);
            if (target == null) {
                Projectile.ai[0] = 1f;
                if (Projectile.velocity.Length() < 7f) {
                    Projectile.velocity *= 1.02f;
                }
                return;
            }

            Projectile.ai[1]++;
            bool charge = Projectile.ai[1] > 360f;
            float speed = 8f;
            if (Projectile.ai[0] > 0f && charge) {
                Projectile.ai[0] = 1f;
                speed = 12f;
            }

            var targetCenter = target.Center;
            var toTarget = Projectile.DirectionTo(targetCenter);
            if (charge || Projectile.Distance(targetCenter) > 170f) {
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, toTarget * speed, 0.1f);
            }
            else {
                Projectile.velocity *= 0.97f;
            }

            Projectile.ai[0] = 0f;
            if (!charge && (int)Projectile.ai[1] % 40 == 0) {
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    Projectile.Center,
                    toTarget * 13f,
                    ProjectileID.HornetStinger,
                    Projectile.damage,
                    Projectile.knockBack,
                    Projectile.owner
                );
            }
            if (charge && Projectile.ai[0] < 1f) {
                Projectile.ai[0]++;
            }
        }

        public override bool PreDraw(ref Color lightColor) {
            return true;
        }
    }
}