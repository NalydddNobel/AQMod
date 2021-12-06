using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Monster
{
    public class SuperStarite : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 25;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 8;
            projectile.height = 8;
            projectile.hostile = true;
            projectile.aiStyle = -1;
            projectile.timeLeft = 300;
            projectile.extraUpdates = 5;
        }

        public override void AI()
        {
            if (projectile.ai[0] <= 0f)
            {
                projectile.ai[0] = projectile.ai[0] * -1 + 1f;
                projectile.velocity *= 0.2f;
                projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
            }
            switch (projectile.ai[0])
            {
                case 2:
                projectile.velocity *= 0.99f;
                if (projectile.velocity.Length() < 0.25f)
                {
                    projectile.timeLeft = 2;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        for (int i = 0; i < 5; i++)
                        {

                        }
                    }
                }
                break;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Vector2 offset = new Vector2(projectile.width / 2f, projectile.height / 2f) - Main.screenPosition;
            Color color = AQMod.CosmicEvent.stariteProjectileColor * 0.4f;
            float mult = 1f / ProjectileID.Sets.TrailCacheLength[projectile.type];
            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
            {
                float progress = mult * i;
                spriteBatch.Draw(Main.projectileTexture[projectile.type], projectile.oldPos[i] + offset, null, Color.Lerp(color * 10f, color, progress) * (1f - progress) * ModContent.GetInstance<AQConfigClient>().EffectIntensity, projectile.rotation, Main.projectileTexture[projectile.type].Size() / 2f, projectile.scale - progress, SpriteEffects.None, 0f);
            }
            spriteBatch.Draw(Main.projectileTexture[projectile.type], projectile.position + offset, null, AQMod.CosmicEvent.stariteProjectileColor, projectile.rotation, Main.projectileTexture[projectile.type].Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(Main.projectileTexture[projectile.type], projectile.position + offset, null, AQMod.CosmicEvent.stariteProjectileColor * 0.1f, projectile.rotation, Main.projectileTexture[projectile.type].Size() / 2f, projectile.scale + 0.1f, SpriteEffects.None, 0f);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Dig, (int)projectile.position.X, (int)projectile.position.Y, 1, 0.35f, 2.5f);
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDust(projectile.position, projectile.width, projectile.height, 15, projectile.velocity.X, projectile.velocity.Y);
            }
            for (int i = 0; i < 8; i++)
            {
                Dust.NewDust(projectile.position, projectile.width, projectile.height, 15, -projectile.velocity.X * 1.25f, -projectile.velocity.Y * 1.25f);
            }
        }
    }
}