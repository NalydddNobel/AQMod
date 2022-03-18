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
                if (target != -1)
                {
                    var targetCenter = Main.npc[target].Center;
                    float distance = (projectile.Center - targetCenter).Length();
                    float amount = MathHelper.Clamp(distance / 1000f, 0.005f, 0.75f);
                    if (projectile.ai[0] > 0f)
                    {
                        projectile.ai[0]--;
                        if (projectile.ai[0] < 0f)
                        {
                            projectile.ai[0] = 0f;
                        }
                        amount = MathHelper.Lerp(amount, 0.0001f, Math.Min(projectile.ai[0] / 40f, 1f));
                    }
                    if (distance < 320f)
                    {
                        projectile.velocity = Vector2.Lerp(projectile.velocity, Vector2.Normalize(targetCenter - projectile.Center) * ((320f - distance) / 10f + 8f), amount);
                    }
                    else
                    {
                        projectile.velocity = Vector2.Lerp(projectile.velocity, Vector2.Normalize(targetCenter - projectile.Center) * 8f, amount);
                    }
                }
                else
                {
                    var idlePos = IdlePosition(player, aQPlayer);
                    float distance = (projectile.Center - idlePos).Length();
                    if (distance > 2000f)
                    {
                        projectile.Center = idlePos;
                        projectile.velocity *= 0.1f;
                    }
                    else if (distance > 20f)
                    {
                        projectile.velocity = Vector2.Lerp(projectile.velocity, (idlePos - projectile.Center) / 32f, 0.01f);
                    }
                }
                projectile.rotation += projectile.velocity.Length() * 0.0157f;
            }
            else
            {
                projectile.velocity = Vector2.Zero;
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
            float set = 60f;
            if (aQPlayer.snowsawLeader != -1 && Main.projectile[aQPlayer.snowsawLeader].type == projectile.type)
            {
                if (Main.projectile[aQPlayer.snowsawLeader].ai[0] < set)
                {
                    Main.projectile[aQPlayer.snowsawLeader].ai[0] = set;
                    Main.projectile[aQPlayer.snowsawLeader].netUpdate = true;
                }
            }
            if (projectile.ai[0] < set)
                projectile.ai[0] = set;
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