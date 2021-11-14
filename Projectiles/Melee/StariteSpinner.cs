using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Melee
{
    public class StariteSpinner : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.YoyosLifeTimeMultiplier[projectile.type] = 10f;
            ProjectileID.Sets.YoyosMaximumRange[projectile.type] = 280f;
            ProjectileID.Sets.YoyosTopSpeed[projectile.type] = 16.5f;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.extraUpdates = 0;
            projectile.width = 20;
            projectile.height = 20;
            projectile.aiStyle = 99;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.melee = true;
            projectile.scale = 1f;
        }

        public override void PostAI()
        {
            if (Main.rand.NextBool(5))
                Dust.NewDust(projectile.position, projectile.width, projectile.height, 15);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = Main.projectileTexture[projectile.type];
            Vector2 origin = texture.Size() / 2f;
            Vector2 drawPos = projectile.Center - Main.screenPosition;
            float mult = 1f / ProjectileID.Sets.TrailCacheLength[projectile.type];
            var offset = new Vector2(projectile.width / 2f, projectile.height / 2f);
            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
            {
                Main.spriteBatch.Draw(texture, projectile.oldPos[i] + offset - Main.screenPosition, null, new Color(60, 60, 60, 0) * (mult * (ProjectileID.Sets.TrailCacheLength[projectile.type] - i)), projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0f);
            }
            Main.spriteBatch.Draw(texture, drawPos, null, new Color(255, 255, 255, 255), projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}