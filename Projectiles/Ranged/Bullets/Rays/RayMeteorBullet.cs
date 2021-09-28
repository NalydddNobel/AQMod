using AQMod.Content.Dusts;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Ranged.Bullets.Rays
{
    public class RayMeteorBullet : RayBullet
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 20;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override Color GetColor() => new Color(255, 155, 135, 5);

        public override void AI()
        {
            int targetIndex = -1;
            float distance = 50f;
            var center = projectile.Center;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].CanBeChasedBy())
                {
                    float dist = (Main.npc[i].Center - center).Length() - (float)Math.Sqrt(Main.npc[i].width * Main.npc[i].width + Main.npc[i].height * Main.npc[i].height);
                    if (dist < distance)
                    {
                        targetIndex = i;
                        distance = dist;
                    }
                }
            }
            if (targetIndex != -1)
            {
                projectile.tileCollide = false;
                projectile.timeLeft = 60;
                projectile.velocity = Vector2.Lerp(projectile.velocity, Vector2.Normalize(Main.npc[targetIndex].Center - projectile.Center) * (Main.npc[targetIndex].velocity.Length() * 0.5f + 8f), 0.1f);
            }
            projectile.localAI[1]++;
            if (projectile.localAI[1] > 6f && projectile.hide)
            {
                projectile.hide = false;
            }
            if (!projectile.hide)
            {
                projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
                int dustType = ModContent.DustType<MonoDust>();
                var dustColor = GetColor();
                Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, dustType, 0f, 0f, 0, dustColor, 1.25f)].velocity *= 0.015f;
                projectile.localAI[0]++;
                if (projectile.localAI[0] > 20f)
                {
                    projectile.localAI[0] = 0f;
                    int count = 10;
                    float r = MathHelper.TwoPi / count;
                    for (int i = 0; i < count; i++)
                    {
                        int d = Dust.NewDust(center, 2, 2, dustType, 0f, 0f, 0, dustColor, 0.75f);
                        Main.dust[d].velocity = new Vector2(4f, 0f).RotatedBy(r * i);
                        Main.dust[d].velocity.X *= 0.2f;
                        Main.dust[d].velocity = Main.dust[d].velocity.RotatedBy(projectile.rotation - MathHelper.PiOver2);
                    }
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Main.PlaySound(SoundID.Dig, projectile.Center);
            if (projectile.penetrate > 0)
            {
                if (projectile.velocity.X != oldVelocity.X)
                {
                    projectile.velocity.X = -oldVelocity.X;
                }
                if (projectile.velocity.Y != oldVelocity.Y)
                {
                    projectile.velocity.Y = -oldVelocity.Y;
                }
                projectile.penetrate--;
                return false;
            }
            return true;
        }
    }
}