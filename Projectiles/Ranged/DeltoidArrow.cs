using AQMod.Assets;
using AQMod.Common;
using AQMod.Common.Utilities;
using AQMod.Content.Dusts;
using AQMod.Effects.ScreenEffects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Ranged
{
    public class DeltoidArrow : ModProjectile
    {
        public static int ArrowProjectileIDToHamaYumiProjectileID(int type) // TODO add more arrow types
        {
            return ModContent.ProjectileType<DeltoidArrow>();
        }

        public override void SetDefaults()
        {
            projectile.width = 8;
            projectile.height = 8;
            projectile.aiStyle = -1;
            projectile.ranged = true;
            projectile.friendly = true;
            projectile.timeLeft = 600;
            projectile.extraUpdates = 3;
            projectile.penetrate = 2;
            projectile.hide = true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            projectile.Kill();
        }

        public void SpawnArrowPortal(int x, int y, int ydir = 1)
        {
            int p = Projectile.NewProjectile(new Vector2((x * 16f) + 8, (y * 16f) - (8f * ydir)), new Vector2(projectile.velocity.X * 0.6f, -10f * ydir), (int)projectile.ai[1], projectile.damage, projectile.knockBack, projectile.owner);
            Main.projectile[p].coldDamage = projectile.coldDamage;
            Main.projectile[p].noDropItem = true;
        }

        public override void AI()
        {
            var center = projectile.Center;
            Lighting.AddLight(center, new Vector3(1f, 0.65f, 0.2f));
            if (projectile.velocity.X.Abs() > projectile.velocity.Y.Abs())
            projectile.ai[0]++;
            if (projectile.ai[0] > 35f)
            {
                projectile.ai[0] = 0f;
                var projTile = center.ToTileCoordinates();
                int y = -1;
                for (int i = 0; i < 20; i++)
                {
                    if (AQWorldGen.ActiveAndSolid(projTile.X, projTile.Y + i))
                    {
                        y = projTile.Y + i;
                        break;
                    }
                }
                if (y != -1)
                {
                    SpawnArrowPortal(projTile.X, y);
                }
                y = -1;
                for (int i = 0; i < 20; i++)
                {
                    if (AQWorldGen.ActiveAndSolid(projTile.X, projTile.Y - i))
                    {
                        y = projTile.Y - i;
                        break;
                    }
                }
                if (y != -1)
                {
                    SpawnArrowPortal(projTile.X, y, -1);
                }
            }
            projectile.velocity.Y += 0.03f;
            projectile.localAI[1]++;
            if (projectile.hide)
            {
                if (projectile.localAI[1] > 8f)
                    projectile.hide = false;
            }
            else if (!projectile.hide)
                projectile.rotation = projectile.velocity.ToRotation();
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return AQItem.GetAlphaDemonSiegeWeapon(lightColor);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            lightColor = projectile.GetAlpha(lightColor);
            var texture = TextureCache.GetProjectile(projectile.type);
            var textureOrig = new Vector2(texture.Width / 2f, 10f);
            var offset = new Vector2(projectile.width / 2f, projectile.height / 2f);
            float rotation = projectile.rotation + MathHelper.PiOver2;
            float intensity = AQMod.EffectIntensity;
            if (projectile.timeLeft < 30)
                intensity *= 30 - projectile.timeLeft;
            Main.spriteBatch.Draw(ModContent.GetTexture(CommonUtils.GetPath(this) + "_Aura"), projectile.position + offset - Main.screenPosition, null, new Color(60, 2, 100, 0) * intensity, rotation, textureOrig, projectile.scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, projectile.position + offset - Main.screenPosition, null, lightColor, rotation, textureOrig, projectile.scale, SpriteEffects.None, 0f);
            if (intensity > 10f)
            {
                intensity -= 10f;
                var spotlightTexture = TextureCache.Lights[Assets.Textures.LightID.Spotlight30x30];
                var spotlightTextureOrigin = spotlightTexture.Size() / 2f;
                Main.spriteBatch.Draw(ModContent.GetTexture(CommonUtils.GetPath(this) + "_Aura"), projectile.position + offset - Main.screenPosition, null, new Color(20, 8, 50, 0) * intensity, rotation, textureOrig, projectile.scale, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(spotlightTexture, projectile.position + offset - Main.screenPosition, null, new Color(60, 10, 100, 0) * intensity, rotation, spotlightTextureOrigin, projectile.scale * intensity / 8f, SpriteEffects.None, 0f);
                if (AQMod.EffectQuality >= 1f)
                    Main.spriteBatch.Draw(spotlightTexture, projectile.position + offset - Main.screenPosition, null, new Color(10, 2, 40, 0) * (intensity / 5f), rotation, spotlightTextureOrigin, projectile.scale * intensity / 3, SpriteEffects.None, 0f);
            }
            return false;
        }

        public override void Kill(int timeLeft)
        {
            var clr = new Color(200, 150, 80, 0);
            int type = ModContent.DustType<MonoDust>();
            for (int i = 0; i < 20; i++)
            {
                int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, type, 0, 0, 0, clr * Main.rand.NextFloat(0.8f, 2f), Main.rand.NextFloat(0.8f, 2f));
                Main.dust[d].velocity *= 1.2f;
            }
        }
    }

    public class DeltoidArrowSpawn : ModProjectile
    {
        public override string Texture => AQMod.ModName + "/" + TextureCache.None;

        public override void SetDefaults()
        {
            projectile.width = 8;
            projectile.height = 16;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.aiStyle = -1;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.timeLeft = 20;
            projectile.ignoreWater = true;
            projectile.usesIDStaticNPCImmunity = true;
            projectile.idStaticNPCHitCooldown = 24;
        }
    }
}