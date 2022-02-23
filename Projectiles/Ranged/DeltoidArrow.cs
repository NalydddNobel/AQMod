using AQMod.Assets;
using AQMod.Common.ID;
using AQMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
            projectile.width = 10;
            projectile.height = 10;
            projectile.aiStyle = -1;
            projectile.ranged = true;
            projectile.friendly = true;
            projectile.timeLeft = 600;
            projectile.extraUpdates = 2;
            projectile.penetrate = 5;
            projectile.alpha = 240;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Buffs.Debuffs.CrimsonHellfire.Inflict(target, 120);
        }

        public override void AI()
        {
            if (Main.netMode == NetmodeID.Server)
            {
                return;
            }
            var center = projectile.Center;
            if (projectile.alpha > 0)
            {
                projectile.alpha -= 5;
                if (projectile.alpha < 0)
                {
                    projectile.alpha = 0;
                }
            }
            float opacity = 1f - projectile.alpha / 255f;
            Lighting.AddLight(center, new Vector3(1f, 0.65f, 0.2f) * opacity);
            int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Fire);
            Main.dust[d].noGravity = true;
            Main.dust[d].velocity *= 0.05f;
            Main.dust[d].alpha = projectile.alpha;
            Main.dust[d].scale = opacity * Main.rand.NextFloat(0.4f, 1.1f);
            projectile.rotation = projectile.velocity.ToRotation();
            if (Main.myPlayer != projectile.owner)
            {
                return;
            }
            if (projectile.localAI[0] == 0f)
            {
                int projectileType = (int)projectile.ai[1];
                if (projectileType == ProjectileID.HolyArrow)
                {
                    projectile.ai[0] = 60f;
                }
                else
                {
                    projectile.ai[0] = 30f;
                }
                if (projectileType == ProjectileID.HellfireArrow)
                {
                    projectile.velocity /= 2f;
                }
                projectile.localAI[0] = projectile.velocity.Length() * 3f;
                if (projectileType == ProjectileID.JestersArrow)
                {
                    projectile.velocity *= 2f;
                }
            }
            projectile.ai[0]--;
            if ((int)projectile.ai[0] <= 0)
            {
                int projectileType = (int)projectile.ai[1];
                if (projectileType == ProjectileID.HolyArrow)
                {
                    projectile.ai[0] = 120f;
                }
                else
                {
                    projectile.ai[0] = 40f;
                }
                projectile.netUpdate = true;
                projectile.localAI[1]++;
                if ((int)projectile.localAI[1] > 5)
                {
                    projectile.ai[0] = 3000f;
                }
                var soundID = SoundID.Item5;
                Main.PlaySound(soundID.SoundId, (int)center.X, (int)center.Y, soundID.Style, 0.8f, 0.6f);
                int p = Projectile.NewProjectile(center, Vector2.Normalize(projectile.velocity) * projectile.localAI[0], projectileType, projectile.damage, projectile.knockBack, projectile.owner);
                Main.projectile[p].rotation = projectile.rotation;
                if (Main.projectile[p].penetrate != -1 && Main.projectile[p].penetrate < projectile.penetrate)
                {
                    Main.projectile[p].penetrate = projectile.penetrate;
                    Main.projectile[p].usesIDStaticNPCImmunity = true;
                    Main.projectile[p].idStaticNPCHitCooldown = 12;
                }
                Main.projectile[p].coldDamage = projectile.coldDamage;
                Main.projectile[p].noDropItem = true;
            }
            projectile.velocity.Y += 0.03f;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return AQItem.Commons.DemonSiegeItem_GetAlpha(lightColor);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            lightColor = projectile.GetAlpha(lightColor);
            lightColor *= 1f - projectile.alpha / 255f;
            var texture = TextureGrabber.GetProjectile(projectile.type);
            var textureOrig = new Vector2(texture.Width / 2f, 10f);
            var offset = new Vector2(projectile.width / 2f, projectile.height / 2f);
            float rotation = projectile.rotation + MathHelper.PiOver2;
            float intensity = AQConfigClient.c_EffectIntensity;
            if (projectile.timeLeft < 30)
                intensity *= 30 - projectile.timeLeft;
            Main.spriteBatch.Draw(ModContent.GetTexture(AQUtils.GetPath(this) + "_Aura"), projectile.position + offset - Main.screenPosition, null, new Color(60, 2, 100, 0) * intensity, rotation, textureOrig, projectile.scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, projectile.position + offset - Main.screenPosition, null, lightColor, rotation, textureOrig, projectile.scale, SpriteEffects.None, 0f);
            if (intensity > 10f)
            {
                intensity -= 10f;
                var spotlightTexture = LegacyTextureCache.Lights[LightTex.Spotlight30x30];
                var spotlightTextureOrigin = spotlightTexture.Size() / 2f;
                Main.spriteBatch.Draw(ModContent.GetTexture(AQUtils.GetPath(this) + "_Aura"), projectile.position + offset - Main.screenPosition, null, new Color(20, 8, 50, 0) * intensity, rotation, textureOrig, projectile.scale, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(spotlightTexture, projectile.position + offset - Main.screenPosition, null, new Color(60, 10, 100, 0) * intensity, rotation, spotlightTextureOrigin, projectile.scale * intensity / 8f, SpriteEffects.None, 0f);
                if (AQConfigClient.c_EffectQuality >= 1f)
                    Main.spriteBatch.Draw(spotlightTexture, projectile.position + offset - Main.screenPosition, null, new Color(10, 2, 40, 0) * (intensity / 5f), rotation, spotlightTextureOrigin, projectile.scale * intensity / 3, SpriteEffects.None, 0f);
            }
            return false;
        }

        public override void Kill(int timeLeft)
        {
            int type = ModContent.DustType<MonoDust>();
            var center = projectile.Center;
            float r = projectile.Size.Length() / 3f;
            for (int i = 0; i < 20; i++)
            {
                var clr = new Color(255, 100 + Main.rand.Next(-20, 20), 20 + Main.rand.Next(-20, 30), 0);
                int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, type, 0, 0, 0, clr * Main.rand.NextFloat(0.8f, 2f), Main.rand.NextFloat(0.8f, 2f));
                var normal = new Vector2(1f, 0f).RotatedBy(Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi));
                Main.dust[d].position = center + normal * Main.rand.NextFloat(r);
                Main.dust[d].velocity = normal * Main.rand.NextFloat(0.65f, 3f);
            }
        }
    }
}