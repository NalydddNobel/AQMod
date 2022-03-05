using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;

namespace AQMod.Projectiles.Summon
{
    public sealed class SnowsawMinion : AQMinion
    {
        private float _pulseTimer;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            projectile.netImportant = true;
            projectile.width = 40;
            projectile.height = 40;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.minionSlots = 1;
            projectile.penetrate = -1;
            projectile.timeLeft *= 5;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.manualDirectionChange = true;
            projectile.minionSlots = 0.5f;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 60;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(155, 155, 155, 100);
        }

        public override Vector2 IdlePosition(Player player, AQPlayer aQPlayer)
        {
            return base.IdlePosition(player, aQPlayer) + new Vector2(0f, -100f);
        }

        public override bool MinionContactDamage()
        {
            return true;
        }

        public override void AI()
        {
            projectile.localNPCHitCooldown = 60;
            var player = Main.player[projectile.owner];
            var aQPlayer = player.GetModPlayer<AQPlayer>();
            ActiveCheck(ref aQPlayer.snowsaw, player);
            _pulseTimer = Main.GlobalTime;
            if (aQPlayer.snowsawLeader == -1)
            {
                aQPlayer.snowsawLeader = projectile.whoAmI;
                projectile.ai[1] = 0f;
                projectile.rotation += 0.01125f;

                int target = FindTarget(1500f, useForceTarget: true, tileLineCheckDistanceMultiplier: 2f);
                var idlePos = IdlePosition(player, aQPlayer);
                if (target != -1)
                {
                    if (projectile.ai[0] > 60f)
                    {
                        if ((int)projectile.ai[0] < 62)
                        {
                            projectile.velocity = (Main.npc[target].Center - projectile.Center) / 16f;
                            if (projectile.velocity.Length() < 18f)
                            {
                                projectile.velocity.Normalize();
                                projectile.velocity *= 18f;
                            }
                        }
                        if (projectile.ai[0] >= 100)
                        {
                            if (projectile.ai[0] >= 150f)
                            {
                                float distance = (projectile.Center - idlePos).Length();
                                SnapTo(idlePos, 0.12f);
                                if (distance < 24f || projectile.ai[0] > 200f)
                                {
                                    projectile.ai[0] = 1f;
                                }
                            }
                            else
                            {
                                projectile.rotation += 0.6f - (projectile.ai[0] - 100f) * 0.012f;
                                projectile.localNPCHitCooldown = 6 + (int)((projectile.ai[0] - 100f) * 0.24f);
                                SnapTo(idlePos, 0.001f);
                            }
                        }
                        else
                        {
                            projectile.rotation += 0.6f;
                        }
                    }
                    else
                    {
                        projectile.rotation += projectile.ai[0] * 0.01f;
                        SnapTo(idlePos, 0.08f - projectile.ai[0] * 0.0012f);
                    }
                    projectile.ai[0]++;
                }
                else
                {
                    if (projectile.ai[0] > 0f)
                    {
                        if (projectile.ai[0] >= 150f)
                        {
                            float distance = (projectile.Center - idlePos).Length();
                            SnapTo(idlePos, 0.12f);
                            if (distance < 24f)
                            {
                                projectile.ai[0] = 0f;
                            }
                        }
                        else if (projectile.ai[0] >= 100f)
                        {
                            projectile.rotation += 0.6f - (projectile.ai[0] - 100f) * 0.012f;
                            SnapTo(idlePos, 0.001f);
                            projectile.ai[0]++;
                        }
                        else
                        {
                            projectile.ai[0] = 0f;
                        }
                    }
                    else
                    {
                        SnapTo(idlePos, 0.08f);
                    }
                }
            }
            else
            {
                projectile.ai[0] = 0f;
                int index = (int)Main.projectile[aQPlayer.snowsawLeader].ai[1];
                int outwards = (4 + index) / 8 + 1;
                projectile.rotation = Main.projectile[aQPlayer.snowsawLeader].rotation;
                float rot = projectile.rotation;
                if (index >= 4)
                {
                    float rot2 = 0.15f * ((index - 4) / 8f + 1);
                    rot2 = (index - 4) / 4 % 2 == 1 ? -rot2 : rot2;
                    rot += rot2;
                    projectile.rotation += rot2;
                }
                projectile.Center = Main.projectile[aQPlayer.snowsawLeader].Center + new Vector2((projectile.width - 6f) * outwards * projectile.scale, 0f).RotatedBy(index * MathHelper.PiOver2 + rot);
                Main.projectile[aQPlayer.snowsawLeader].ai[1]++;
                _pulseTimer -= (Main.projectile[aQPlayer.snowsawLeader].Center - projectile.Center).Length() * 0.15f;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            var player = Main.player[projectile.owner];
            var aQPlayer = player.GetModPlayer<AQPlayer>();
            if (aQPlayer.snowsawLeader != -1 && Main.projectile[aQPlayer.snowsawLeader].type == projectile.type)
            {
                if (Main.projectile[aQPlayer.snowsawLeader].ai[0] < 99f)
                {
                    Main.projectile[aQPlayer.snowsawLeader].ai[0] = 99f;
                    Main.projectile[aQPlayer.snowsawLeader].netUpdate = true;
                }
            }
            if (projectile.ai[0] < 99f)
                projectile.ai[0] = 99f;
            target.AddBuff(BuffID.Frostburn, 120);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            var texture = Main.projectileTexture[projectile.type];
            var frame = texture.Frame();
            var origin = frame.Size() / 2f;
            var offset = new Vector2(projectile.width / 2f, projectile.height / 2f) - Main.screenPosition;
            var color = projectile.GetAlpha(lightColor);
            int trailLength = ProjectileID.Sets.TrailCacheLength[projectile.type];
            for (int i = 0; i < trailLength; i++)
            {
                float progress = 1f / trailLength * i;
                Main.spriteBatch.Draw(texture, projectile.oldPos[i] + offset, frame, new Color(0, 0, 120, 0) * (1f - progress), projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0f);
            }
            Main.spriteBatch.Draw(texture, projectile.position + offset, frame, color, projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0f);
            float wave = AQUtils.Wave(_pulseTimer * 4f, 0f, 4f);
            color *= 0.1f + wave * 0.1f;
            color.A = 0;
            for (int i = 0; i < 4; i++)
            {
                Main.spriteBatch.Draw(texture, projectile.position + offset + new Vector2(wave, 0f).RotatedBy(MathHelper.PiOver2 * i + projectile.rotation), frame, color, projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0f);
            }
            return false;
        }
    }
}