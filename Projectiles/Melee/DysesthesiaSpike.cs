using AQMod.Assets;
using AQMod.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Melee
{
    public class DysesthesiaSpike : ModProjectile
    {
        private const int MaxDistance = 10;

        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.aiStyle = -1;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.melee = true;
            projectile.scale = 1.2f;
            projectile.alpha = 255;
        }

        public override void AI()
        {
            int owner = AQProjectile.FindIdentityAndType((int)projectile.ai[0], ModContent.ProjectileType<Dysesthesia>());
            if (owner == -1)
            {
                projectile.Kill();
                return;
            }

            projectile.Center = Main.projectile[owner].Center;

            if ((int)projectile.ai[1] >= 0)
                projectile.ai[1] += projectile.ai[1] * 0.1f + 0.0025f;

            if ((int)projectile.ai[1] > MaxDistance * projectile.scale)
            {
                projectile.ai[1] = -1f;
                projectile.timeLeft = 40;
            }

            if ((int)projectile.ai[1] <= -1)
            {
                projectile.position += projectile.velocity * (MaxDistance * projectile.scale * (1f - projectile.timeLeft / 40));
                return;
            }

            if ((int)projectile.ai[1] < 6)
            {
                projectile.alpha = 255 - (int)(projectile.ai[1] * (255f / 6f));
            }
            else
            {
                projectile.alpha = 0;
            }

            projectile.position += projectile.velocity * projectile.ai[1];
            projectile.rotation = projectile.velocity.ToRotation();
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Main.rand.NextBool(4) && target.Distance(Main.player[projectile.owner].Center) < (ProjectileID.Sets.YoyosMaximumRange[projectile.type] / 2f))
                target.AddBuff(ModContent.BuffType<Buffs.Debuffs.CorruptionHellfire>(), 360);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            var texture = this.GetTexture();
            var origin = new Vector2(texture.Width, 2f);
            var clr = lightColor * ((255 - projectile.alpha) / 255);
            Main.spriteBatch.Draw(texture, projectile.Center + projectile.velocity * projectile.width / 2f - Main.screenPosition, null, clr, projectile.rotation, origin, new Vector2(projectile.scale, 0.8f), SpriteEffects.None, 0f);
            return false;
        }
    }
}