using AQMod.Dusts;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Melee
{
    public class HorizonSkies : ModProjectile
    {
        public static Color BlueDustColoring => new Color(144, 144, 255, 128);
        public static Color OrangeDustColoring => new Color(150, 110, 66, 128);

        public override void SetDefaults()
        {
            projectile.width = 18;
            projectile.height = 18;
            projectile.melee = true;
            projectile.friendly = true;
            projectile.aiStyle = 19;
            projectile.penetrate = -1;
            projectile.alpha = 0;
            projectile.hide = true;
            projectile.ownerHitCheck = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Player projOwner = Main.player[projectile.owner];
            Vector2 ownerMountedCenter = projOwner.RotatedRelativePoint(projOwner.MountedCenter, true);
            projectile.direction = projOwner.direction;
            projOwner.heldProj = projectile.whoAmI;
            projOwner.itemTime = projOwner.itemAnimation;
            projectile.position.X = ownerMountedCenter.X - projectile.width / 2;
            projectile.position.Y = ownerMountedCenter.Y - projectile.height / 2;
            if (!projOwner.frozen)
            {
                if (projectile.ai[0] == 0f)
                {
                    projectile.ai[0] = 2f;
                    projectile.velocity = projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.05f, 0.05f));
                    projectile.netUpdate = true;
                }
                if (projOwner.itemAnimation < projOwner.itemAnimationMax / 3)
                {
                    projectile.ai[0] -= 2.4f;
                }
                else
                {
                    projectile.ai[0] += 0.9f;
                }
            }
            projectile.position += projectile.velocity * projectile.ai[0];
            if (projOwner.itemAnimation < projOwner.itemAnimationMax / 5 * 3)
            {
                if (projectile.ai[1] == 0f)
                {
                    Vector2 center = projectile.Center;
                    float rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
                    Projectile.NewProjectile(center, projectile.velocity * 1.2f, ModContent.ProjectileType<HorizonSkiesBolt>(), projectile.damage, projectile.knockBack, projectile.owner, MathHelper.PiOver2);
                    for (int i = 0; i < 30; i++)
                    {
                        int d = Dust.NewDust(center + new Vector2((float)Math.Sin(i) * 10f, 0f).RotatedBy(rotation), 1, 1, ModContent.DustType<MonoDust>(), 0f, 0f, 0, BlueDustColoring);
                        Main.dust[d].velocity += projectile.velocity;
                        Main.dust[d].noGravity = true;
                        d = Dust.NewDust(center + new Vector2((float)Math.Cos(i) * 10f, 0f).RotatedBy(rotation), 1, 1, ModContent.DustType<MonoDust>(), 0f, 0f, 0, OrangeDustColoring);
                        Main.dust[d].velocity += projectile.velocity;
                        Main.dust[d].noGravity = true;
                    }
                    Main.PlaySound(SoundID.Trackable, (int)ownerMountedCenter.X, (int)ownerMountedCenter.Y, 28 + Main.rand.Next(3), 0.5f, -1f);
                }
                projectile.ai[1] = 1f;
            }
            if (projOwner.itemAnimation == 0)
                projectile.Kill();
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver4 * 3f;
            if (projectile.spriteDirection == -1)
                projectile.rotation -= MathHelper.PiOver2;
        }
    }
}
