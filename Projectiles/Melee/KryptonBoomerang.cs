using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Melee
{
    public class KryptonBoomerang : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.extraUpdates = 4;
            projectile.manualDirectionChange = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 1200;
        }

        public override void AI()
        {
            if ((int)projectile.ai[0] == 0)
            {
                projectile.direction = projectile.velocity.X < 0f ? -1 : 1;
            }
            projectile.ai[0]++;
            if (projectile.ai[0] > 60f)
            {
                projectile.tileCollide = false;
                float speed = Math.Max((Main.player[projectile.owner].Center - projectile.Center).Length() / 140f, 10f);
                projectile.velocity = Vector2.Lerp(projectile.velocity, Vector2.Normalize(Main.player[projectile.owner].Center - projectile.Center) * speed, Math.Max(1f - (Main.player[projectile.owner].Center - projectile.Center).Length() / 50f, 0.01f));
                if ((projectile.Center - Main.player[projectile.owner].Center).Length() < 20f)
                {
                    projectile.Kill();
                }
            }
            var dustPosition = projectile.Center + new Vector2(projectile.width - 4f, 0f).RotatedBy(MathHelper.PiOver2 * -projectile.direction + projectile.velocity.ToRotation());
            int d = Dust.NewDust(dustPosition, 2, 2, ModContent.DustType<Dusts.NobleMushrooms.KryptonDust>());
            Main.dust[d].velocity *= 0.1f;
            projectile.rotation += 0.125f;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
        {
            width = 4;
            height = 4;
            fallThrough = true;
            return true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            projectile.velocity = oldVelocity;
            projectile.ai[0] = 220f;
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(250, 250, 250, 250);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Main.rand.NextBool(4))
            {
                target.AddBuff(BuffID.Confused, 120);
            }
            if (Main.rand.NextBool(4))
            {
                target.AddBuff(BuffID.Poisoned, 120);
            }
        }
    }
}