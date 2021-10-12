using AQMod.Common.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles
{
    public class JerryPearl : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 64;
            projectile.height = 64;
            projectile.aiStyle = -1;
            projectile.hostile = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 900;
        }

        public override void AI()
        {
            if (projectile.wet)
            {
                var player = Main.player[Player.FindClosest(projectile.position, projectile.width, projectile.height)];
                if (projectile.Center.Y < player.Center.Y)
                {
                    projectile.velocity.Y += 0.25f;
                    if (projectile.velocity.Y > 16f)
                        projectile.velocity.Y = 16f;
                }
                else
                {
                    projectile.velocity.Y -= 0.25f;
                    if (projectile.velocity.Y < -16f)
                        projectile.velocity.Y = -16f;
                }
            }
            else
            {
                projectile.velocity.Y += 0.25f;
                if (projectile.velocity.Y > 8f)
                    projectile.velocity.Y = 8f;
            }
            if (projectile.velocity.X.Abs() < 2f)
            {
                projectile.velocity.X *= 0.98f;
                if (projectile.timeLeft > 60)
                    projectile.timeLeft = 60;
            }
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
        {
            fallThrough = Main.player[Player.FindClosest(projectile.position, projectile.width, projectile.height)].position.Y
                > projectile.position.Y + projectile.height;
            return true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (projectile.velocity.Y != oldVelocity.Y && oldVelocity.Y > 2f)
            {
                projectile.velocity.Y = -oldVelocity.Y * 1.1f;
                if (projectile.wet)
                {
                    if (projectile.velocity.Y < -16f)
                        projectile.velocity.Y = -16f;
                }
                else
                {
                    if (projectile.velocity.Y < -8f)
                        projectile.velocity.Y = -8f;
                    Main.PlaySound(SoundID.Item10, projectile.position);
                }
            }
            if (projectile.velocity.X != oldVelocity.X)
                projectile.velocity.X = -oldVelocity.X * 0.9f;
            return false;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Shatter, projectile.position);
        }
    }
}