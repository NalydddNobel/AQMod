using AQMod.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Minions
{
    public class StariteMinion : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projPet[projectile.type] = true;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 21;
            ProjectileID.Sets.Homing[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.netImportant = true;
            projectile.width = 28;
            projectile.height = 28;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.penetrate = -1;
            projectile.timeLeft *= 5;
            projectile.usesIDStaticNPCImmunity = true;
            projectile.idStaticNPCHitCooldown = 6;
        }

        public override bool MinionContactDamage() => true;

        public override void AI()
        {
            int leader = (int)projectile.ai[0] - 1;

            if (leader < 0 || !Main.projectile[leader].active || Main.projectile[leader].type != ModContent.ProjectileType<StariteMinionLeader>())
            {
                projectile.Kill();
            }
            else
            {
                projectile.timeLeft = 2;
            }

            var player = Main.player[projectile.owner];

            if (!projectile.tileCollide)
            {
                var difference = Main.player[projectile.owner].Center - projectile.Center;
                if (difference.Length() < 10f || Collision.CanHitLine(projectile.position, projectile.width, projectile.height, player.position, player.width, player.height))
                {
                    projectile.tileCollide = true;
                }
                Vector2 lerpedVelocity = Vector2.Normalize(Vector2.Lerp(projectile.velocity, difference, 0.1f)) * Math.Max(6f, projectile.velocity.Length());
                projectile.velocity = lerpedVelocity;
                return;
            }

            var leaderProjectile = Main.projectile[leader];

            if ((int)leaderProjectile.ai[0] == 0f)
            {
                float turnSpeed = MathHelper.Clamp(leaderProjectile.ai[1] / 10000f, 0f, 1f);
                if (turnSpeed != 0f)
                {
                    var center = projectile.Center;
                    int target = -1;
                    float dist = 2000f;
                    if (player.HasMinionAttackTargetNPC)
                    {
                        var difference = projectile.Center - center;
                        if ((float)Math.Sqrt(difference.X * difference.X + difference.Y * difference.Y) < dist && Collision.CanHitLine(projectile.position, projectile.width, projectile.height, projectile.position, projectile.width, projectile.height))
                        {
                            target = player.MinionAttackTargetNPC;
                        }
                    }
                    if (target == -1)
                    {
                        for (int i = 0; i < Main.maxNPCs; i++)
                        {
                            NPC NPC = Main.npc[i];
                            if (NPC.CanBeChasedBy())
                            {
                                var difference = projectile.Center - center;
                                float c = (float)Math.Sqrt(difference.X * difference.X + difference.Y * difference.Y);
                                if (Collision.CanHitLine(projectile.position, projectile.width, projectile.height, projectile.position, projectile.width, projectile.height))
                                    c *= 2;
                                if (c < dist)
                                {
                                    target = i;
                                    dist = c;
                                }
                            }
                        }
                    }

                    Vector2 gotoPosition = target == -1 ? Main.player[projectile.owner].Center : Main.npc[target].Center;

                    projectile.ai[1] = target;

                    float length = leaderProjectile.velocity.Length();
                    var difference2 = gotoPosition - center;
                    Vector2 lerpedVelocity = Vector2.Normalize(Vector2.Lerp(projectile.velocity, difference2, turnSpeed)) * length;
                    projectile.velocity = lerpedVelocity;
                }
            }
            else if ((int)leaderProjectile.ai[0] == 1f && leaderProjectile.localAI[0] != 0f && leaderProjectile.localAI[1] != 0f)
            {
                projectile.ai[1] = -1;
                var gotoVelo = new Vector2(leaderProjectile.localAI[0], leaderProjectile.localAI[1]);
                float length = gotoVelo.Length();
                projectile.velocity = Vector2.Normalize(Vector2.Lerp(projectile.velocity, gotoVelo, 0.08f)) * length;
            }

            if ((int)projectile.ai[1] == -1)
            {
                if (!Collision.CanHitLine(projectile.position, projectile.width, projectile.height, player.position, player.width, player.height))
                {
                    projectile.tileCollide = false;
                }
            }
        }

        public override void PostAI()
        {
            int leaderType = ModContent.ProjectileType<StariteMinionLeader>();
            float size = new Vector2(projectile.width, projectile.height).Length();
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (projectile.whoAmI != i && Main.projectile[i].active && (Main.projectile[i].type == projectile.type || Main.projectile[i].type == leaderType))
                {
                    var difference = (projectile.Center - Main.projectile[i].Center);
                    float length = difference.Length();
                    if (length < size)
                    {
                        projectile.velocity += difference * 0.01f;
                    }
                }
            }
            if ((projectile.Center - Main.player[projectile.owner].Center).Length() > 2000f)
            {
                projectile.Center = Main.player[projectile.owner].Center;
            }
            if (Main.rand.NextBool(40))
            {
                int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, 15);
                Main.dust[d].velocity = projectile.velocity * 0.01f;
            }
            if (Main.rand.NextBool(80))
            {
                int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, 58);
                Main.dust[d].velocity.X = Main.rand.NextFloat(-4f, 4f);
                Main.dust[d].velocity.Y = Main.rand.NextFloat(-4f, 4f);
            }
            if (Main.rand.NextBool(80))
            {
                int g = Gore.NewGore(projectile.position + new Vector2(Main.rand.Next(projectile.width - 4), Main.rand.Next(projectile.height - 4)), new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f)), 16 + Main.rand.Next(2));
                Main.gore[g].scale *= 0.6f;
            }
            Lighting.AddLight(projectile.Center, new Vector3(0.2f, 0.2f, 0.1f));
            projectile.rotation += projectile.velocity.Length() * 0.0157f;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (projectile.velocity.X != oldVelocity.X)
            {
                if (oldVelocity.X.Abs() > 2f)
                    projectile.velocity.X = -oldVelocity.X * 0.8f;
                projectile.localAI[0] *= -0.8f;
            }
            if (projectile.velocity.Y != oldVelocity.Y)
            {
                if (oldVelocity.Y.Abs() > 2f)
                    projectile.velocity.Y = -oldVelocity.Y * 0.8f;
                projectile.localAI[1] *= -0.8f;
            }
            return true;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(250, 250, 250, 250);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            lightColor = GetAlpha(lightColor).GetValueOrDefault(lightColor);
            Texture2D texture = Main.projectileTexture[projectile.type];
            Rectangle frame = new Rectangle(0, 0, texture.Width, texture.Height);
            Vector2 offset = new Vector2(projectile.width / 2, projectile.height / 2);
            var effects = projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            var color = new Color(lightColor.R, lightColor.G, lightColor.B, lightColor.A) * 0.25f;
            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
            {
                float progress = 1f / ProjectileID.Sets.TrailCacheLength[projectile.type] * i;
                Main.spriteBatch.Draw(texture, projectile.oldPos[i] + offset - Main.screenPosition, frame, color * (1f - progress), projectile.rotation, frame.Size() / 2f, Math.Max(projectile.scale * (1f - progress), 0.1f), effects, 0f);
            }
            Main.spriteBatch.Draw(texture, projectile.position + offset - Main.screenPosition, frame, lightColor, projectile.rotation, frame.Size() / 2f, 1f, projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            return false;
        }
    }

    public sealed class StariteMinionLeader : StariteMinion
    {
        public override string Texture => "AQMod/Projectiles/Minions/StariteMinion";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            projectile.minionSlots = 1f;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            var aQPlayer = player.GetModPlayer<AQPlayer>();
            var drawingPlayer = player.GetModPlayer<AQVisualsPlayer>();
            var center = projectile.Center;
            if (player.dead)
                aQPlayer.stariteMinion = false;
            if (aQPlayer.stariteMinion)
                projectile.timeLeft = 2;

            if (!projectile.tileCollide)
            {
                var difference = Main.player[projectile.owner].Center - projectile.Center;
                if (difference.Length() < 10f || Collision.CanHitLine(projectile.position, projectile.width, projectile.height, player.position, player.width, player.height))
                {
                    projectile.tileCollide = true;
                }
                Vector2 lerpedVelocity = Vector2.Normalize(Vector2.Lerp(projectile.velocity, difference, 0.1f)) * Math.Max(6f, projectile.velocity.Length());
                projectile.velocity = lerpedVelocity;
                return;
            }

            int target = -1;
            float dist = 2000f;
            if (player.HasMinionAttackTargetNPC)
            {
                var difference = projectile.Center - center;
                if ((float)Math.Sqrt(difference.X * difference.X + difference.Y * difference.Y) < dist && Collision.CanHitLine(projectile.position, projectile.width, projectile.height, projectile.position, projectile.width, projectile.height))
                {
                    target = player.MinionAttackTargetNPC;
                }
            }
            if (target == -1)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC NPC = Main.npc[i];
                    if (NPC.CanBeChasedBy())
                    {
                        var difference = projectile.Center - center;
                        float c = (float)Math.Sqrt(difference.X * difference.X + difference.Y * difference.Y);
                        if (Collision.CanHitLine(projectile.position, projectile.width, projectile.height, projectile.position, projectile.width, projectile.height))
                            c *= 2;
                        if (c < dist)
                        {
                            target = i;
                            dist = c;
                        }
                    }
                }
            }

            Vector2 gotoPosition = target == -1 ? player.Center : Main.npc[target].Center;

            if ((int)projectile.ai[0] == 0f)
            {
                if (projectile.ai[1] == 0f)
                {
                    projectile.ai[1] = Main.rand.Next(30, 70);
                    projectile.netUpdate = true;
                }
                if (projectile.ai[1] == 1f)
                {
                    projectile.ai[0] = 1f;
                    projectile.netUpdate = true;
                }
                float turnSpeed = MathHelper.Clamp(projectile.ai[1] / 10000f, 0f, 1f);
                projectile.ai[1]--;
                if (projectile.localAI[0] > 0)
                    projectile.localAI[0]--;
                if (turnSpeed != 0f)
                {
                    float length = projectile.velocity.Length();
                    Vector2 difference = gotoPosition - center;
                    Vector2 lerpedVelocity = Vector2.Normalize(Vector2.Lerp(projectile.velocity, difference, turnSpeed)) * length;
                    projectile.velocity = lerpedVelocity;
                }
            }
            else if ((int)projectile.ai[0] == 1f)
            {
                if (projectile.localAI[0] == 0f && projectile.localAI[1] == 0f)
                {
                    var gotoVelo = new Vector2(Main.rand.NextFloat(4f, 6.5f) + 2f, 0f).RotatedBy((gotoPosition - center).ToRotation());
                    projectile.localAI[0] = gotoVelo.X;
                    projectile.localAI[1] = gotoVelo.Y;
                }
                else
                {
                    var gotoVelo = new Vector2(projectile.localAI[0], projectile.localAI[1]);
                    float length = gotoVelo.Length();
                    projectile.velocity = Vector2.Normalize(Vector2.Lerp(projectile.velocity, gotoVelo, 0.08f)) * length;
                    bool xCloseEnough = (projectile.velocity.X - gotoVelo.X).Abs() < 0.1f;
                    bool yCloseEnough = (projectile.velocity.Y - gotoVelo.Y).Abs() < 0.1f;
                    if (xCloseEnough && yCloseEnough)
                    {
                        projectile.velocity.X = gotoVelo.X;
                        projectile.velocity.Y = gotoVelo.Y;
                        projectile.ai[0] = 0f;
                        projectile.localAI[0] = 0f;
                        projectile.localAI[1] = 0f;
                        projectile.netUpdate = true;
                    }
                }
            }

            if (target == -1)
            {
                if (!Collision.CanHitLine(projectile.position, projectile.width, projectile.height, player.position, player.width, player.height))
                {
                    projectile.tileCollide = false;
                }
            }
        }
    }
}