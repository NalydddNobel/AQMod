using AQMod.Common.Utilities;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;

namespace AQMod.Projectiles.Summon.StariteMinion
{
    public sealed class StariteLeader : Starite
    {
        public override string Texture => AQUtils.GetPath<Starite>();

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
            var drawingPlayer = player.GetModPlayer<AQPlayer>();
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
