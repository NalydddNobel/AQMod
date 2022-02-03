using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Monster
{
    public class SoldierCrabSandBall : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_" + ProjectileID.SandBallGun;

        public override void SetDefaults()
        {
            projectile.width = 8;
            projectile.height = 8;
            projectile.aiStyle = -1;
            projectile.hostile = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 120;
        }

        public override void AI()
        {
            projectile.rotation += 0.0315f;
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
                projectile.velocity.Y = -oldVelocity.Y * 0.33f;
            if (projectile.velocity.X != oldVelocity.X)
                projectile.velocity.X = -oldVelocity.X * 0.33f;
            return false;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (Main.rand.NextBool(8))
            {
                target.AddBuff(ModContent.BuffType<Buffs.Debuffs.PickBreak>(), 480);
            }
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Dig, projectile.position);
        }
    }
}