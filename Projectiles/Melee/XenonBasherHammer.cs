using AQMod.Dusts.NobleMushrooms;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Melee
{
    public class XenonBasherHammer : HammerProjectile
    {
        protected override int Size => 48;
        protected override float LengthFromPlayer => 55f;
        protected override float UseTime => 15f;
        protected override Vector2 ModifyPlayerOffset(Vector2 offset)
        {
            return new Vector2(offset.X, offset.Y - 32f);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(250, 250, 250, 250);
        }

        protected override void OnImpactGround()
        {
            base.OnImpactGround();
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y + projectile.height - 2f), projectile.width, 2, ModContent.DustType<XenonDust>());
            }
            for (int i = 0; i < 2; i++)
            {
                Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y + projectile.height - 2f), projectile.width, 2, ModContent.DustType<XenonMist>());
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            float mushroomSpeed = knockback / 2f;
            var spawnPosition = target.Center;
            int type = ModContent.ProjectileType<XenonBasher>();
            float speed = 6f * Main.player[projectile.owner].meleeSpeed;
            int amount = Main.rand.Next(3) + 1;
            if (crit)
                amount = (int)(amount * 2.5f);
            for (int i = 0; i < amount; i++)
            {
                Projectile.NewProjectile(spawnPosition, new Vector2(speed, 0f).RotatedBy(Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi)), type, damage, mushroomSpeed, projectile.owner);
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