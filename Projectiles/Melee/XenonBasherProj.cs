using AQMod.Dusts.NobleMushrooms;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Melee
{
    public class XenonBasherProj : ModProjectile
    {
        public override string Texture => Tex.None;

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.timeLeft = 8;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.aiStyle = -1;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.ownerHitCheck = true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            float mushroomSpeed = knockback / 2f;
            var spawnPosition = target.Center;
            int type = ModContent.ProjectileType<XenonBasherSpore>();
            float speed = 6f * Main.player[projectile.owner].meleeSpeed;
            int amount = Main.rand.Next(3) + 1;
            if (crit)
                amount = (int)(amount * 2.5f);
            for (int i = 0; i < amount; i++)
            {
                Projectile.NewProjectile(spawnPosition, new Vector2(speed, 0f).RotatedBy(Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi)), type, damage / 3, mushroomSpeed, projectile.owner);
            }
            spawnPosition += new Vector2(-6f, -6f);
            type = ModContent.DustType<XenonDust>();
            for (int i = 0; i < amount * 10; i++)
            {
                Main.dust[Dust.NewDust(spawnPosition, 8, 8, type)].velocity = new Vector2(10f, 0f).RotatedBy(Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi));
            }
            type = ModContent.DustType<XenonMist>();
            for (int i = 0; i < amount; i++)
            {
                Main.dust[Dust.NewDust(spawnPosition, 8, 8, type)].velocity = new Vector2(10f, 0f).RotatedBy(Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi));
            }
        }
    }
}