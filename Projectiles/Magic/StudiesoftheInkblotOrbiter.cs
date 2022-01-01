using AQMod.Assets;
using AQMod.Common.Graphics.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Magic
{
    public class StudiesoftheInkblotOrbiter : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[projectile.type] = 7;
        }

        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 32;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.timeLeft = 80;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.scale = 0.8f;
            projectile.extraUpdates = 4;
            projectile.penetrate = -1;
        }

        public override bool CanDamage()
        {
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return StudiesoftheInkblotBullet.GetProjectileColor(projectile.frame);
        }

        public static Vector2 GetPosition(float X, float Y, float T, Vector2 playerOrigin)
        {
            return playerOrigin + new Vector2((float)Math.Sin(T) * X, (float)Math.Cos(T) * Y);
        }

        public static Vector2 GetVelocityNormal(Projectile projectile)
        {
            return Vector2.Normalize(GetPosition(projectile.ai[0] + 0.01f, projectile.localAI[0], projectile.localAI[1], Main.player[projectile.owner].Center) - projectile.Center);
        }

        public static int X = 100;
        public static int Y = 215;

        public static void Spawn4(int plr)
        {
            var center = Main.player[plr].Center;
            int damage = Main.player[plr].HeldItem.damage;
            float kb = Main.player[plr].HeldItem.knockBack;
            int type = ModContent.ProjectileType<StudiesoftheInkblotOrbiter>();
            int p = Projectile.NewProjectile(center, Vector2.Zero, type, damage, kb, plr);
            Main.projectile[p].localAI[0] = X;
            Main.projectile[p].localAI[1] = Y;
            p = Projectile.NewProjectile(center, Vector2.Zero, type, damage, kb, plr);
            Main.projectile[p].localAI[1] = X;
            Main.projectile[p].localAI[0] = Y;
            p = Projectile.NewProjectile(center, Vector2.Zero, type, damage, kb, plr, MathHelper.Pi);
            Main.projectile[p].localAI[0] = X;
            Main.projectile[p].localAI[1] = Y;
            p = Projectile.NewProjectile(center, Vector2.Zero, type, damage, kb, plr, MathHelper.Pi);
            Main.projectile[p].localAI[1] = X;
            Main.projectile[p].localAI[0] = Y;
        }

        public override void AI()
        {
            int time = (int)Main.GameUpdateCount / 120;
            if (Main.player[projectile.owner].HeldItem.type == ModContent.ItemType<Items.Weapons.Magic.StudiesoftheInkblot>())
                projectile.timeLeft = 4;

            if (Main.myPlayer == projectile.owner)
            {
                if ((int)projectile.localAI[0] == 0)
                {
                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        if (Main.projectile[i].active && Main.projectile[i].type == projectile.type && Main.projectile[i].owner == projectile.owner)
                        {
                            if (i % 2 == 1)
                            {
                                projectile.localAI[0] = 215f;
                                projectile.localAI[1] = 100f;
                            }
                            else
                            {
                                projectile.localAI[0] = 100f;
                                projectile.localAI[1] = 215f;
                            }
                        }
                    }
                }
                projectile.Center = Main.player[projectile.owner].Center + new Vector2((float)Math.Sin(projectile.ai[0]) * projectile.localAI[0], (float)Math.Cos(projectile.ai[0]) * projectile.localAI[1]);
            }
            if (projectile.localAI[0] == Y)
            {
                time += 120;
            }
            projectile.frame = time % 7;
            projectile.ai[0] += 0.01f;
            var color = projectile.GetAlpha(default(Color)) * projectile.scale;
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Particle.PreDrawProjectiles.AddParticle(new BrightSparkle(projectile.RandomPosition(8), projectile.velocity * -0.1f, color, Main.rand.NextFloat(0.7f, 0.9f)));
            }
            Lighting.AddLight(projectile.Center, color.ToVector3());
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (projectile.alpha > 255)
            {
                return false;
            }
            float colorMultiplier = 1 - projectile.alpha / 255f;
            float colorMultiplierSquared = colorMultiplier * colorMultiplier;
            var texture = Main.projectileTexture[projectile.type];
            var offset = new Vector2(projectile.width / 2f, projectile.height / 2f);
            var spotlight = AQTextures.Lights[LightTex.Spotlight66x66];
            spriteBatch.Draw(spotlight, projectile.position + offset - Main.screenPosition, null, projectile.GetAlpha(lightColor) * colorMultiplierSquared, projectile.rotation, spotlight.Size() / 2f, projectile.scale * 0.6f, SpriteEffects.None, 0f);
            var frame = texture.Frame(1, Main.projFrames[projectile.type], 0, projectile.frame);
            spriteBatch.Draw(texture, projectile.position + offset - Main.screenPosition, frame, new Color(255, 255, 255, 255) * colorMultiplierSquared, projectile.rotation, frame.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}