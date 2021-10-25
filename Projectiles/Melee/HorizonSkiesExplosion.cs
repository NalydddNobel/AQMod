using AQMod.Content.Dusts;
using AQMod.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Melee
{
    public class HorizonSkiesExplosion : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 80;
            projectile.height = 80;
            projectile.melee = true;
            projectile.aiStyle = -1;
            projectile.friendly = true;
            projectile.timeLeft = 5;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.manualDirectionChange = true;
            projectile.hide = true;
        }

        public override void AI()
        {
            var center = projectile.Center;
            int d = Dust.NewDust(center + new Vector2((float)Math.Sin(projectile.ai[0]) * 25f, 0f).RotatedBy(projectile.rotation + MathHelper.PiOver2), 1, 1, ModContent.DustType<MonoDust>(), 0f, 0f, 0, Items.Weapons.Melee.HorizonSkies.Blue);
            Main.dust[d].noGravity = true;
            d = Dust.NewDust(center + new Vector2((float)Math.Cos(projectile.ai[0]) * 25f, 0f).RotatedBy(projectile.rotation + MathHelper.PiOver2), 1, 1, ModContent.DustType<MonoDust>(), 0f, 0f, 0, Items.Weapons.Melee.HorizonSkies.Orange);
            Main.dust[d].noGravity = true;
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            hitDirection = projectile.direction;
        }
    }
}