using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Monster
{
    public class MagmabubbleProj : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 6;
            Projectile.hostile = true;
            Projectile.timeLeft = 300;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(180, 180, 180, 0);
        }

        public override void AI()
        {
            if (Main.rand.NextBool(4))
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch);
            if (Projectile.lavaWet)
            {
                var player = Main.player[Player.FindClosest(Projectile.position, Projectile.width, Projectile.height)];
                if (Projectile.Center.Y < player.Center.Y)
                {
                    Projectile.velocity.Y += 0.25f;
                    if (Projectile.velocity.Y > 16f)
                        Projectile.velocity.Y = 16f;
                }
                else
                {
                    Projectile.velocity.Y -= 0.25f;
                    if (Projectile.velocity.Y < -16f)
                        Projectile.velocity.Y = -16f;
                }
            }
            else
            {
                Projectile.velocity.Y += 0.25f;
                if (Projectile.velocity.Y > 8f)
                    Projectile.velocity.Y = 8f;
            }
            if (Projectile.velocity.X.Abs() < 2f)
            {
                Projectile.velocity.X *= 0.98f;
                if (Projectile.timeLeft > 60)
                    Projectile.timeLeft = 60;
            }
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = Main.player[Player.FindClosest(Projectile.position, Projectile.width, Projectile.height)].position.Y
                > Projectile.position.Y + Projectile.height;
            return true;
        }

        private void CollisionEffects(Vector2 velocity)
        {
            Vector2 spawnPos = Projectile.position + velocity;
            for (int i = 0; i < 5; i++)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch);
                Main.dust[d].velocity = new Vector2(0f, 2f).RotatedBy(Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi));
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            bool doEffects = false;
            if (oldVelocity.X != Projectile.velocity.X && oldVelocity.Y.Abs() > 2f)
            {
                doEffects = true;
                Projectile.position.X += Projectile.velocity.X * 0.9f;
                Projectile.velocity.X = -oldVelocity.X * 0.8f;
            }
            if (oldVelocity.Y != Projectile.velocity.Y && oldVelocity.Y.Abs() > 2f)
            {
                doEffects = true;
                Projectile.position.Y += Projectile.velocity.Y * 0.9f;
                Projectile.velocity.Y = -oldVelocity.Y * 0.8f;
            }
            if (doEffects)
            {
                CollisionEffects(Projectile.velocity);
            }
            else
            {
                Projectile.velocity *= 0.95f;
            }
            return false;
        }

        public override void Kill(int timeLeft)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                return;
            }

            SoundEngine.PlaySound(SoundID.Item85.WithPitchVariance(2f), Projectile.position);
            for (int i = 0; i < 18; i++)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch);
                Main.dust[d].velocity = new Vector2(0f, 3f).RotatedBy(Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi));
            }
        }
    }
}