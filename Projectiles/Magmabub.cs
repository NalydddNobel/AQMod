using AQMod.Common.Utilities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles
{
    public class Magmabub : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 6;
            projectile.height = 6;
            projectile.hostile = true;
            projectile.timeLeft = 300;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(180, 180, 180, 0);
        }

        public override void AI()
        {
            if (Main.rand.NextBool(4))
                Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Fire);
            if (projectile.lavaWet)
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

        private void CollisionEffects(Vector2 velocity)
        {
            Vector2 spawnPos = projectile.position + velocity;
            for (int i = 0; i < 5; i++)
            {
                int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Fire);
                Main.dust[d].velocity = new Vector2(0f, 2f).RotatedBy(Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi));
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            bool doEffects = false;
            if (oldVelocity.X != projectile.velocity.X && oldVelocity.Y.Abs() > 2f)
            {
                doEffects = true;
                projectile.position.X += projectile.velocity.X * 0.9f;
                projectile.velocity.X = -oldVelocity.X * 0.8f;
            }
            if (oldVelocity.Y != projectile.velocity.Y && oldVelocity.Y.Abs() > 2f)
            {
                doEffects = true;
                projectile.position.Y += projectile.velocity.Y * 0.9f;
                projectile.velocity.Y = -oldVelocity.Y * 0.8f;
            }
            if (doEffects)
                CollisionEffects(projectile.velocity);
            else
            {
                projectile.velocity *= 0.95f;
            }
            return false;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item85.WithPitchVariance(2f), projectile.position);
            for (int i = 0; i < 18; i++)
            {
                int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Fire);
                Main.dust[d].velocity = new Vector2(0f, 3f).RotatedBy(Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi));
            }
        }
    }
}