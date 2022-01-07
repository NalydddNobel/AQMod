using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Summon
{
    public class TrapperMinionBlast : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionShot[projectile.type] = true;
            ProjectileID.Sets.Homing[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 18;
            projectile.height = 18;
            projectile.friendly = true;
            projectile.minion = true;
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
            Main.dust[d].velocity = Vector2.Lerp(-projectile.velocity * 0.3f, Main.dust[d].velocity.RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f)), 0.5f);
            Main.dust[d].scale = Main.rand.NextFloat(0.9f, 2f);
            Main.dust[d].noGravity = true;
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (Main.rand.NextBool(12))
            {
                d = Dust.NewDust(projectile.position, projectile.width, projectile.height,
                    ModContent.DustType<Dusts.MonoSparkleDust>(), 0f, 0f, 0, new Color(255, 100 + Main.rand.Next(-20, 150), 10, 0));
                Main.dust[d].velocity = new Vector2(0f, -Main.rand.NextFloat(3f, 5f)).RotatedBy(Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi));
                Main.dust[d].noGravity = true;
            }
            if (projectile.ai[0] > 0f)
            {
                projectile.ai[0]--;
                projectile.tileCollide = false;
                return;
            }
            projectile.tileCollide = true;
            projectile.velocity.Y += 0.5f;
            if (projectile.velocity.Y > 20f)
                projectile.velocity.Y = 20f;
            if (projectile.velocity.X.Abs() < 1f)
            {
                projectile.velocity.X *= 0.98f;
                if (projectile.timeLeft > 60)
                    projectile.timeLeft = 60;
            }
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
        {
            fallThrough = true;
            return true;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item14, projectile.position);
            int p = Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<TrapperMinionExplosion>(), projectile.damage, projectile.knockBack, projectile.owner);
            Vector2 position = projectile.Center - new Vector2(Main.projectile[p].width / 2f, Main.projectile[p].height / 2f);
            Main.projectile[p].position = position;
            var bvelo = -projectile.velocity * 0.4f;
            for (int i = 0; i < 4; i++)
            {
                int d = Dust.NewDust(Main.projectile[p].position, Main.projectile[p].width, Main.projectile[p].height,
                    ModContent.DustType<Dusts.MonoSparkleDust>(), 0f, 0f, 0, new Color(255, 100, 10, 0));
                Main.dust[d].velocity = new Vector2(0f, -Main.rand.NextFloat(4f, 7.5f)).RotatedBy(MathHelper.PiOver2 * i + Main.rand.NextFloat(-0.1f, 0.1f));
                Main.dust[d].noGravity = true;
            }
            for (int i = 0; i < 4; i++)
            {
                int d = Dust.NewDust(Main.projectile[p].position, Main.projectile[p].width, Main.projectile[p].height, 31);
                Main.dust[d].velocity = Vector2.Lerp(bvelo, Main.dust[d].velocity, 0.7f);
                Main.dust[d].noGravity = true;
            }
            for (int i = 0; i < 16; i++)
            {
                int d = Dust.NewDust(Main.projectile[p].position, Main.projectile[p].width, Main.projectile[p].height, DustID.Fire);
                Main.dust[d].scale = Main.rand.NextFloat(0.9f, 2f);
                Main.dust[d].velocity = Vector2.Lerp(bvelo, Main.dust[d].velocity, 0.7f);
                Main.dust[d].noGravity = true;
            }
        }
    }
}
