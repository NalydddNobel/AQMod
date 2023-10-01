using Aequus;
using Aequus.Common.DataSets;
using Aequus.Common.Particles;
using Aequus.Content;
using Aequus.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Misc.CrownOfBlood {
    public class ThermiteGel : ModProjectile {
        public override void SetStaticDefaults() {
            ProjectileID.Sets.TrailCacheLength[Type] = 6;
            ProjectileID.Sets.TrailingMode[Type] = 1;
            PushableEntities.AddProj(Type);
            ProjectileSets.DealsHeatDamage.Add(Type);
        }

        public override void SetDefaults() {
            Projectile.width = 80;
            Projectile.height = 80;
            Projectile.penetrate += 2;
            Projectile.timeLeft = 300;
            //Projectile.extraUpdates = 1;
        }

        public override void AI() {
            Projectile.velocity.Y += 0.2f;
            Projectile.rotation += Projectile.velocity.Length() / 40f;
            var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Blood, -Projectile.velocity.X * 0.3f, -Projectile.velocity.Y * 0.3f);
            d.noGravity = true;
            Lighting.AddLight(Projectile.Center, Color.Red.ToVector3() * Helper.Wave(Main.GlobalTimeWrappedHourly * 5f, 0.8f, 1f));
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac) {
            width = 20;
            height = 20;
            fallThrough = true;
            return true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity) {
            Projectile.penetrate--;
            if (Projectile.penetrate == 0) {
                Projectile.Kill();
                return false;
            }
            int target = Projectile.FindTargetWithLineOfSight(800f);
            if (target != -1) {
                Projectile.velocity = Projectile.DirectionTo(Main.npc[target].Center).UnNaN() * oldVelocity.Length() * 0.9f;
                return false;
            }

            if (oldVelocity.Length() > 2f) {
                Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            }

            if (Projectile.velocity.X != oldVelocity.X && oldVelocity.X.Abs() > 2f) {
                Projectile.velocity.X = -oldVelocity.X * 0.9f;
            }
            if (Projectile.velocity.Y != oldVelocity.Y && oldVelocity.Y.Abs() > 2f) {
                Projectile.velocity.Y = -oldVelocity.Y * 0.9f;
            }
            return false;
        }

        public override void OnKill(int timeLeft) {
            if (Main.myPlayer != Projectile.owner) {
                return;
            }

            Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Vector2.Normalize(Projectile.velocity), ModContent.ProjectileType<ThermiteGelExplosion>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }

        public override bool PreDraw(ref Color lightColor) {
            Projectile.GetDrawInfo(out var t, out var off, out var frame, out var origin, out int trailLength);

            Main.spriteBatch.Draw(t, Projectile.position + off - Main.screenPosition, frame, lightColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(t, Projectile.position + off - Main.screenPosition, frame, Color.White, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);

            var trailColor = new Color(30, 30, 30, 0);
            for (int i = 0; i < trailLength; i++) {
                float p = Helper.CalcProgress(trailLength, i);
                Main.spriteBatch.Draw(t, Projectile.oldPos[i] + off - Main.screenPosition, frame, trailColor * p, Projectile.oldRot[i], origin, Projectile.scale * (0.8f + 0.2f * p), SpriteEffects.None, 0f);
            }
            return false;
        }
    }

    public class ThermiteGelExplosion : ModProjectile {
        public override string Texture => "Aequus/Assets/Explosion1";

        public override void SetStaticDefaults() {
            Main.projFrames[Type] = 7;
        }

        public override void SetDefaults() {
            Projectile.DefaultToExplosion(90, DamageClass.Ranged, 20);
        }

        public override Color? GetAlpha(Color lightColor) {
            return new Color(255, 70, 128, 128);
        }

        public override void AI() {
            if (Projectile.frame == 0 && Main.netMode != NetmodeID.Server) {
                for (int i = 0; i < 8; i++) {
                    var v = Main.rand.NextVector2Unit();
                    ParticleSystem.New<MonoBloomParticle>(ParticleLayer.BehindPlayers).Setup(Projectile.Center + v * Main.rand.NextFloat(16f), v * Main.rand.NextFloat(3f, 12f),
                        new Color(255, 15, 25, 0), new Color(20, 5, 10, 0), 1.25f, 0.3f);
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