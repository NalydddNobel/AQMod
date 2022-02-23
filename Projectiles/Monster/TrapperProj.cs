using AQMod.Assets;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Monster
{
    public class TrapperProj : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 18;
            projectile.height = 18;
            projectile.hostile = true;
            projectile.aiStyle = -1;
            projectile.timeLeft = 300;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(180, 180, 180, 0);
        }

        public override void AI()
        {
            int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Fire);
            Main.dust[d].velocity = Vector2.Lerp(projectile.velocity, Main.dust[d].velocity, 0.5f);
            Main.dust[d].scale = Main.rand.NextFloat(0.9f, 2f);
            Main.dust[d].noGravity = true;
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (projectile.ai[0] > 0f)
            {
                projectile.ai[0]--;
                return;
            }
            if (projectile.lavaWet)
            {
                var player = Main.player[Player.FindClosest(projectile.position, projectile.width, projectile.height)];
                if (projectile.Center.Y < player.Center.Y)
                {
                    projectile.velocity.Y += 1f;
                    if (projectile.velocity.Y > 40f)
                        projectile.velocity.Y = 40f;
                }
                else
                {
                    projectile.velocity.Y -= 1f;
                    if (projectile.velocity.Y < -40f)
                        projectile.velocity.Y = -40f;
                }
            }
            else
            {
                projectile.velocity.Y += 1f;
                if (projectile.velocity.Y > 20f)
                    projectile.velocity.Y = 20f;
            }
            if (projectile.velocity.X.Abs() < 1f)
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

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item14, projectile.position);
            int p = Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<TrapperFireblastExplosion>(), projectile.damage, projectile.knockBack, projectile.owner);
            Vector2 position = projectile.Center - new Vector2(Main.projectile[p].width / 2f, Main.projectile[p].height / 2f);
            Main.projectile[p].position = position;
            var bvelo = -projectile.velocity * 0.4f;
            for (int i = 0; i < 3; i++)
            {
                Gore.NewGore(Main.projectile[p].position, bvelo * 0.2f, 61 + Main.rand.Next(3));
            }
            for (int i = 0; i < 12; i++)
            {
                int d = Dust.NewDust(Main.projectile[p].position, Main.projectile[p].width, Main.projectile[p].height, 31);
                Main.dust[d].velocity = Vector2.Lerp(bvelo, Main.dust[d].velocity, 0.7f);
                Main.dust[d].noGravity = true;
            }
            for (int i = 0; i < 30; i++)
            {
                int d = Dust.NewDust(Main.projectile[p].position, Main.projectile[p].width, Main.projectile[p].height, DustID.Fire);
                Main.dust[d].scale = Main.rand.NextFloat(0.9f, 2f);
                Main.dust[d].velocity = Vector2.Lerp(bvelo, Main.dust[d].velocity, 0.7f);
                Main.dust[d].noGravity = true;
            }
        }
    }

    public class TrapperFireblastExplosion : ModProjectile
    {
        public override string Texture => "AQMod/" + LegacyTextureCache.None;

        public override void SetDefaults()
        {
            projectile.width = 46;
            projectile.height = 46;
            projectile.timeLeft = 2;
            projectile.hostile = true;
            projectile.aiStyle = -1;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
        }

        public override void AI()
        {
            if (projectile.ai[0] > 0)
                projectile.active = false;
            projectile.ai[0]++;
        }
    }
}
