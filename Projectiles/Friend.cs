using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Projectiles
{
    public class Friend : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.timeLeft *= 5;
            projectile.penetrate = -1;
        }

        public override void AI()
        {
            projectile.localAI[0] += 0.0314f;
            if (Main.player[projectile.owner].dead)
                Main.player[projectile.owner].GetModPlayer<AQPlayer>().omori = false;
            if (Main.player[projectile.owner].GetModPlayer<AQPlayer>().omori)
                projectile.timeLeft = 2;
            switch (projectile.ai[0])
            {
                default:
                {
                    projectile.frame = 0;
                    projectile.gfxOffY = (float)Math.Sin(projectile.localAI[0]) * 2f;
                }
                break;

                case 1:
                {
                    projectile.frame = 1;
                    var center = projectile.Center;
                    int target = -1;
                    float closest = 2000f;
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        if (Main.npc[i].CanBeChasedBy())
                        {
                            var dist = (Main.npc[i].Center - center).Length();
                            if (dist < closest)
                            {
                                target = i;
                                closest = dist;
                            }
                        }
                    }
                    if (target != -1)
                    {
                        if (projectile.ai[1] == 0f)
                            projectile.ai[1] = 1f;
                        var npc = Main.npc[target];
                        var dist = (npc.Center - center).Length();
                        if (dist < 20f)
                            projectile.ai[1] = 30f;
                        else if (dist < 200f && projectile.ai[1] <= 1f)
                        {
                            projectile.velocity = Vector2.Lerp(projectile.velocity, Vector2.Normalize(npc.Center - center) * 20f, 0.035f);
                        }
                        else
                        {
                            if (projectile.ai[1] > 1f)
                                projectile.ai[1]--;
                            projectile.velocity = Vector2.Lerp(projectile.velocity, Vector2.Normalize(npc.Center - center) * 20f, 0.0075f);
                        }
                        projectile.rotation += projectile.velocity.Length() * 0.0314f;
                        if (projectile.velocity.X < 0f)
                            projectile.spriteDirection = -1;
                        else
                        {
                            projectile.spriteDirection = 1;
                        }
                    }
                    else
                    {
                        if (projectile.ai[1] > 0f)
                        {
                            var plr = Main.player[projectile.owner];
                            var plrCenter = plr.Center;
                            var gotoPos = plrCenter + new Vector2(0f, -plr.height);
                            projectile.velocity = Vector2.Lerp(projectile.velocity, Vector2.Normalize(plrCenter - center) * 20f, 0.035f);
                            projectile.rotation += projectile.velocity.Length() * 0.0314f;
                            if ((center - plrCenter).Length() < 30f)
                                projectile.ai[1] = 0f;
                        }
                        else
                        {
                            projectile.spriteDirection = Main.player[projectile.owner].direction;
                            projectile.rotation = MathHelper.Lerp(projectile.rotation, 0f, 0.1f);
                            var plr = Main.player[projectile.owner];
                            var plrCenter = plr.Center;
                            var gotoPos = plrCenter + new Vector2(0f, -plr.height);
                            projectile.velocity = Vector2.Lerp(center, gotoPos, 0.1f) - center;
                            projectile.gfxOffY = projectile.velocity.Length() < 2f ? (float)Math.Sin(projectile.localAI[0]) * 2f : 0f;
                        }
                    }
                }
                break;

                case 2:
                {
                    projectile.frame = 2;
                    var center = projectile.Center;
                    var plr = Main.player[projectile.owner];
                    var plrCenter = plr.Center;
                    var gotoPos = plrCenter + new Vector2(plr.height, 0f);
                    projectile.velocity = Vector2.Lerp(center, gotoPos, 0.1f) - center;
                    projectile.gfxOffY = projectile.velocity.Length() < 2f ? (float)Math.Sin(projectile.localAI[0]) * 2f : 0f;
                    int target = -1;
                    float closest = 400f;
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        if (Main.npc[i].CanBeChasedBy())
                        {
                            var dist = (Main.npc[i].Center - center).Length();
                            if (dist < closest)
                            {
                                target = i;
                                closest = dist;
                            }
                        }
                    }
                    if (target != -1)
                    {
                        var npc = Main.npc[target];
                        projectile.spriteDirection = npc.Center.X > center.X ? 1 : -1;
                        if (projectile.ai[1] > 45f)
                        {
                            if (Collision.CanHitLine(projectile.position, projectile.width, projectile.height, npc.position, npc.width, npc.height))
                            {
                                projectile.ai[1] = 0f;
                                Projectile.NewProjectile(center, Vector2.Normalize(npc.Center - center + new Vector2(0f, -npc.height)) * 10f, ModContent.ProjectileType<KelBall>(), projectile.damage, projectile.knockBack, projectile.owner);
                            }
                        }
                        else
                        {
                            projectile.ai[1]++;
                        }
                    }
                    else
                    {
                        projectile.spriteDirection = Main.player[projectile.owner].direction;
                    }
                }
                break;

                case 3:
                {
                    projectile.frame = 3;
                    var center = projectile.Center;
                    var plr = Main.player[projectile.owner];
                    var plrCenter = plr.Center;
                    var gotoPos = plrCenter + new Vector2(-plr.height, 0f);
                    projectile.velocity = Vector2.Lerp(center, gotoPos, 0.1f) - center;
                    projectile.gfxOffY = projectile.velocity.Length() < 2f ? (float)Math.Sin(projectile.localAI[0]) * 2f : 0f;
                    projectile.spriteDirection = Main.player[projectile.owner].direction;
                    if (projectile.ai[1] > 30f)
                    {
                        if (plr.statLife < plr.statLifeMax2)
                        {
                            projectile.ai[1] = 0f;
                            plr.HealEffect(1, false);
                            plr.statLife += 1;
                        }
                    }
                    else
                    {
                        projectile.ai[1]++;
                    }
                }
                break;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            var texture = Main.projectileTexture[projectile.type];
            int frameHeight = texture.Height / Main.projFrames[projectile.type];
            var frame = new Rectangle(0, frameHeight * projectile.frame, texture.Width, frameHeight - 2);
            var effects = SpriteEffects.None;
            if (projectile.spriteDirection == -1)
                effects = SpriteEffects.FlipHorizontally;
            Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, frame, Color.White, projectile.rotation, frame.Size() / 2f, projectile.scale, effects, 0f);
            return false;
        }
    }
}