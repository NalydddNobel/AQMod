using AQMod.Content.Dusts;
using AQMod.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Melee
{
    public class HorizonSkiesBolt : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 40;
            projectile.height = 40;
            projectile.melee = true;
            projectile.aiStyle = -1;
            projectile.friendly = true;
            projectile.penetrate = 5;
            projectile.extraUpdates = 5;
            projectile.hide = true;
        }

        private void Hit(Vector2 position)
        {
            if (projectile.ai[1] > 0f)
                return;
            int p = Projectile.NewProjectile(position, new Vector2(0f, 0f), ModContent.ProjectileType<HorizonSkiesExplosion>(), projectile.damage, projectile.knockBack, projectile.owner);
            Vector2 realPos = new Vector2(position.X - Main.projectile[p].width / 2f, position.Y - Main.projectile[p].height / 2f);
            Main.projectile[p].position = realPos;
            Main.projectile[p].direction = projectile.velocity.X < 0f ? -1 : 1;
            for (int i = 0; i < 50; i++)
            {
                int d = Dust.NewDust(realPos, Main.projectile[p].width, Main.projectile[p].height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, Items.Weapons.Melee.HorizonSkies.Blue);
                Main.dust[d].velocity = Vector2.Normalize(Main.dust[d].position - position) * 10f;
                Main.dust[d].noGravity = true;
                d = Dust.NewDust(realPos, Main.projectile[p].width, Main.projectile[p].height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, Items.Weapons.Melee.HorizonSkies.Orange);
                Main.dust[d].velocity = Vector2.Normalize(Main.dust[d].position - position) * 10f;
                Main.dust[d].noGravity = true;
            }
            Main.PlaySound(SoundID.Item14, realPos);
            projectile.ai[1] = 10f;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
        {
            width = projectile.width / 4;
            height = projectile.height / 4;
            fallThrough = true;
            return true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Hit(target.Center);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            Hit(target.Center);
        }

        public override void Kill(int timeLeft)
        {
            Hit(projectile.Center);
        }

        public override void AI()
        {
            projectile.ai[1]--;
            projectile.velocity.Y += 0.04f;
            projectile.velocity.X *= 0.9999f;
            Vector2 center = projectile.Center;
            int d = Dust.NewDust(center + new Vector2((float)Math.Sin(projectile.ai[0]) * 25f, 0f).RotatedBy(projectile.rotation + MathHelper.PiOver2), 1, 1, ModContent.DustType<MonoDust>(), 0f, 0f, 0, Items.Weapons.Melee.HorizonSkies.Blue);
            Main.dust[d].velocity *= 0.01f;
            Main.dust[d].noGravity = true;
            d = Dust.NewDust(center + new Vector2((float)Math.Sin(projectile.ai[0] + MathHelper.Pi) * 25f, 0f).RotatedBy(projectile.rotation + MathHelper.PiOver2), 1, 1, ModContent.DustType<MonoDust>(), 0f, 0f, 0, Items.Weapons.Melee.HorizonSkies.Orange);
            Main.dust[d].velocity *= 0.01f;
            Main.dust[d].noGravity = true;
            projectile.ai[0] += 0.314f;
            projectile.rotation = projectile.velocity.ToRotation();
        }
    }
}