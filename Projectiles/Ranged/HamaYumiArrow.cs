using AQMod.Assets;
using AQMod.Common.Utilities;
using AQMod.Common.WorldGeneration;
using AQMod.Content.Dusts;
using AQMod.Effects.ScreenEffects;
using AQMod.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Ranged
{
    public class HamaYumiArrow : ModProjectile
    {
        public static int ArrowProjectileIDToHamaYumiProjectileID(int type) // TODO add more arrow types
        {
            return ModContent.ProjectileType<HamaYumiArrow>();
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
            Buffs.Debuffs.CorruptionHellfire.Inflict(target, 120);
            projectile.Kill();
        }

        private void getTarget(out int targetIndex, out float distance)
        {
            targetIndex = -1;
            distance = 100f;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].CanBeChasedBy() && Collision.CanHitLine(projectile.position, projectile.width, projectile.height, Main.npc[i].position, Main.npc[i].width, Main.npc[i].height))
                {
                    float dist = (Main.npc[i].Center - projectile.Center).Length() - (float)Math.Sqrt(Main.npc[i].width * Main.npc[i].width + Main.npc[i].height * Main.npc[i].height);
                    if (dist < distance)
                    {
                        targetIndex = i;
                        distance = dist;
                    }
                }
            }
        }

        public override void AI()
        {
            var center = projectile.Center;
            Lighting.AddLight(center, new Vector3(0.3f, 0.65f, 0.2f));
            if ((int)projectile.ai[0] == -1)
            {
                projectile.extraUpdates = 1;
                projectile.velocity *= 0.89f;
                if (projectile.timeLeft > 30)
                    projectile.timeLeft = 30;
                int dustChance = projectile.timeLeft / 8;
                if (dustChance <= 1 || Main.rand.NextBool(dustChance))
                {
                    var clr = new Color(180, 20, 160, 0);
                    if (projectile.timeLeft < 10)
                        clr = new Color(240, 180, 240, 10);
                    int type = ModContent.DustType<MonoDust>();
                    int d = Dust.NewDust(projectile.position + new Vector2(-24f, -24f), 48 + projectile.width, 48 + projectile.height, type, 0, 0, 0, clr * Main.rand.NextFloat(0.8f, 2f), Main.rand.NextFloat(0.8f, 2f));
                    Main.dust[d].velocity = (center - Main.dust[d].position) / projectile.width;
                }
                return;
            }
            projectile.velocity.Y += 0.05f;
            getTarget(out int targetIndex, out float distance);
            if (targetIndex != -1)
            {
                if (projectile.velocity.Y.Abs() < 4f)
                {
                    float differenceX = center.X - Main.npc[targetIndex].position.X + Main.npc[targetIndex].width / 2f;
                    if (differenceX.Abs() < Main.npc[targetIndex].width)
                        projectile.ai[0] = -1f;
                }
            }
            projectile.localAI[1]++;
            if (projectile.localAI[1] > 8f && projectile.hide)
                projectile.hide = false;
            if (!projectile.hide)
                projectile.rotation = projectile.velocity.ToRotation();
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return AQItem.Similarities.DemonSiegeItem_GetAlpha(lightColor);
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
            Main.spriteBatch.Draw(ModContent.GetTexture(AQUtils.GetPath<HamaYumiArrow>() + "_Aura"), projectile.position + offset - Main.screenPosition, null, new Color(60, 2, 100, 0) * intensity, rotation, textureOrig, projectile.scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, projectile.position + offset - Main.screenPosition, null, lightColor, rotation, textureOrig, projectile.scale, SpriteEffects.None, 0f);
            if (intensity > 10f)
            {
                intensity -= 10f;
                var spotlightTexture = TextureCache.Lights[Assets.Textures.LightTex.Spotlight30x30];
                var spotlightTextureOrigin = spotlightTexture.Size() / 2f;
                Main.spriteBatch.Draw(ModContent.GetTexture(AQUtils.GetPath<HamaYumiArrow>() + "_Aura"), projectile.position + offset - Main.screenPosition, null, new Color(20, 8, 50, 0) * intensity, rotation, textureOrig, projectile.scale, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(spotlightTexture, projectile.position + offset - Main.screenPosition, null, new Color(60, 10, 100, 0) * intensity, rotation, spotlightTextureOrigin, projectile.scale * intensity / 8f, SpriteEffects.None, 0f);
                if (AQMod.EffectQuality >= 1f)
                    Main.spriteBatch.Draw(spotlightTexture, projectile.position + offset - Main.screenPosition, null, new Color(10, 2, 40, 0) * (intensity / 5f), rotation, spotlightTextureOrigin, projectile.scale * intensity / 3, SpriteEffects.None, 0f);
            }
            return false;
        }

        public override void Kill(int timeLeft)
        {
            var clr = new Color(150, 30, 200, 0);
            int type = ModContent.DustType<MonoDust>();
            for (int i = 0; i < 20; i++)
            {
                int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, type, 0, 0, 0, clr * Main.rand.NextFloat(0.8f, 2f), Main.rand.NextFloat(0.8f, 2f));
                Main.dust[d].velocity *= 1.2f;
            }
            getTarget(out int targetIndex, out float distance);
            if (targetIndex != -1)
            {
                var center = new Vector2(Main.npc[targetIndex].position.X + Main.npc[targetIndex].width / 2f, projectile.position.Y + projectile.height / 2f);
                int d = Dust.NewDust(center, 2, 2, type, 0f, 0f, 0, clr, 1.5f);
                float sum = projectile.position.X + projectile.position.Y;
                Main.dust[d].velocity = Vector2.Zero;
                int x = (int)(center.X / 16f);
                int y = (int)(center.Y / 16f);
                const int maxHeight = 9;
                int topY = -maxHeight;
                int bottomY = maxHeight;
                for (int i = 0; i < maxHeight; i++)
                {
                    if (AQWorldGen.ActiveAndFullySolid(x, y - maxHeight + i))
                        topY = -maxHeight + i;
                    if (AQWorldGen.ActiveAndFullySolid(x, y + maxHeight - i))
                        bottomY = maxHeight - i;
                }
                float x2 = x * 16f;
                float x3 = x * 16f + 8f;
                Main.PlaySound(SoundID.Item105.WithVolume(0.4f), projectile.Center);
                for (int yy = y + topY; yy < y + bottomY; yy++)
                {
                    for (int i = 0; i < 7; i++)
                    {
                        d = Dust.NewDust(new Vector2(x2 + 4f, yy * 16f), 8, 16, type, 0, 0, 0, clr * Main.rand.NextFloat(0.8f, 2f), Main.rand.NextFloat(0.8f, 2f));
                        Main.dust[d].velocity *= 0.8f;
                        Main.dust[d].velocity.X *= 0.8f;
                    }
                }
                clr *= 3f;
                for (int i = 0; i < 30; i++)
                {
                    d = Dust.NewDust(new Vector2(x2, (y + topY) * 16f), 16, 4, type, 0, 0, 0, clr, 2f);
                    Main.dust[d].velocity *= 0.2f;
                    Main.dust[d].velocity.Y = Main.rand.NextFloat(0.1f, 1f);
                    d = Dust.NewDust(new Vector2(x2, (y + bottomY) * 16f - 12), 16, 4, type, 0, 0, 0, clr, 2f);
                    Main.dust[d].velocity *= 0.2f;
                    Main.dust[d].velocity.Y = -Main.rand.NextFloat(0.1f, 1f);
                }
                type = ModContent.ProjectileType<HamaYumiExplosion>();
                int height = (y + bottomY - (y + topY)) * 16;
                int p = Projectile.NewProjectile(x3, (y + topY) * 16f, 0f, 0f, type, (int)(projectile.damage * 1.5f), projectile.knockBack, projectile.owner);
                Main.projectile[p].height = height;
                if (Main.myPlayer == projectile.owner && AQMod.TonsofScreenShakes)
                    ScreenShakeManager.AddEffect(new BasicScreenShake(20, AQMod.MultIntensity(2)));
            }
        }
    }

    public class HamaYumiExplosion : ModProjectile
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