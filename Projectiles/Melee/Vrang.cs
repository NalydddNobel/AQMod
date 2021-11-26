using AQMod.Common.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Melee
{
    public class Vrang : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 32;
            projectile.aiStyle = -1;
            projectile.friendly = true;
            projectile.scale = 0.9f;
            projectile.penetrate = -1;
            projectile.melee = true;
            projectile.alpha = 250;
            projectile.timeLeft = 60;
            projectile.penetrate = 3;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
        {
            width = 10;
            height = 10;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough);
        }

        public override void AI()
        {
            if ((int)projectile.ai[0] == -1)
            {
                projectile.alpha = 0;
                projectile.friendly = false;
                projectile.tileCollide = false;
                projectile.velocity.X *= 0.99f;
                if (projectile.velocity.Y < -4f)
                {
                    projectile.velocity.Y *= 0.9f;
                }
                if (projectile.velocity.Y < 16f)
                    projectile.velocity.Y += 0.3f;
                projectile.rotation += projectile.velocity.Length() * 0.02f;
                return;
            }
            projectile.ai[0]++;
            if (projectile.ai[0] > 10f * Main.player[projectile.owner].meleeSpeed)
            {
                if (projectile.velocity.Length() < 22f / Main.player[projectile.owner].meleeSpeed)
                    projectile.velocity += projectile.velocity * 0.08f;

                projectile.localAI[1] += 0.005f;
                projectile.rotation += 0.3f + projectile.localAI[1];
            }
            else
            {
                if (projectile.velocity.Length() > 1f)
                    projectile.velocity *= 0.78f;
                projectile.rotation += 0.3f;
            }
            if (projectile.alpha > 0)
            {
                projectile.alpha -= 25;
                if (projectile.alpha < 0)
                {
                    projectile.alpha = 0;
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Main.PlaySound(SoundID.Dig, (int)projectile.position.X, (int)projectile.position.Y);
            if (projectile.alpha > 100)
            {
                return base.OnTileCollide(oldVelocity);
            }
            bool hitEffect = false;
            if (oldVelocity.X != projectile.velocity.X)
            {
                if (oldVelocity.X.Abs() > 4f)
                    hitEffect = true;
                projectile.position.X += projectile.velocity.X;
                if (oldVelocity.X > oldVelocity.Y)
                    projectile.velocity.X = -oldVelocity.X;
                else
                    projectile.velocity.X = -oldVelocity.X * 0.3f;
            }
            if (oldVelocity.Y != projectile.velocity.Y)
            {
                if (oldVelocity.Y.Abs() > 4f)
                    hitEffect = true;
                projectile.position.Y += projectile.velocity.Y;
                if (oldVelocity.Y > oldVelocity.X)
                    projectile.velocity.Y = -oldVelocity.Y;
                else
                    projectile.velocity.Y = -oldVelocity.Y * 0.3f;
            }
            if (hitEffect)
            {
                projectile.ai[0] = -1f;
                projectile.timeLeft = 120;
                projectile.netUpdate = true;
                projectile.velocity = projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.25f, 0.25f));
                Collision.HitTiles(projectile.position, oldVelocity, projectile.width, projectile.height);
                return false;
            }
            return base.OnTileCollide(oldVelocity);
        }
    }
}