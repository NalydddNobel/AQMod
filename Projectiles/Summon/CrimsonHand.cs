using AQMod.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Summon
{
    public class CrimsonHand : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[projectile.type] = 8;
            Main.projPet[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.Homing[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.netImportant = true;
            projectile.width = 24;
            projectile.height = 20;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.minionSlots = 1;
            projectile.penetrate = -1;
            projectile.timeLeft *= 5;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.usesIDStaticNPCImmunity = true;
            projectile.idStaticNPCHitCooldown = 1;
            projectile.manualDirectionChange = true;
        }

        public override bool? CanCutTiles() => false;
        public override bool MinionContactDamage() => true;
        public override bool CanDamage()
        {
            return projectile.ai[1] > 55f;
        }

        private void GotoPosition(Vector2 gotopos, float amount)
        {
            var lerpPosition = Vector2.Lerp(projectile.Center, gotopos, amount);
            var difference = lerpPosition - projectile.Center;
            projectile.velocity = difference;
            if (difference.X < 0f)
            {
                projectile.direction = -1;
            }
            else
            {
                projectile.direction = 1;
            }
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            var aQPlayer = player.GetModPlayer<AQPlayer>();
            var drawingPlayer = player.GetModPlayer<AQPlayer>();
            var center = projectile.Center;
            if (player.dead)
                aQPlayer.crimsonHands = false;
            if (aQPlayer.crimsonHands)
                projectile.timeLeft = 2;
            int target = -1;
            float distance = 1200f + (50f * (player.ownedProjectileCounts[projectile.type] - 1));

            if (player.HasMinionAttackTargetNPC)
            {
                int t = player.MinionAttackTargetNPC;
                float d = (Main.npc[t].Center - center).Length();
                if (d < distance)
                {
                    target = t;
                    distance = d;
                }
            }

            if (target == -1)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.CanBeChasedBy(attacker: projectile, ignoreDontTakeDamage: false))
                    {
                        var difference = npc.Center - center;
                        float c = (float)Math.Sqrt(difference.X * difference.X + difference.Y * difference.Y);
                        if (Collision.CanHitLine(npc.position, npc.width, npc.height, projectile.position, projectile.width, projectile.height))
                            c *= 2; // enemies behind walls need to be 2x closer in order to be targeted
                        if (c < distance)
                        {
                            target = i;
                            distance = c;
                            if (projectile.ai[0] >= 0f)
                                projectile.ai[0] = -1 - Main.rand.Next(2);
                        }
                    }
                }
            }

            if (target == -1)
            {
                projectile.direction = Main.player[projectile.owner].direction;
                projectile.spriteDirection = projectile.direction;
                int count = 0;
                int index = 0;
                int leaderIndex = 0;
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (i == projectile.whoAmI)
                    {
                        if (count == 0)
                        {
                            leaderIndex = i;
                        }
                        count++;
                        index = count;
                    }
                    else if (Main.projectile[i].active && Main.projectile[i].type == projectile.type && Main.projectile[i].owner == projectile.owner)
                    {
                        if (count == 0)
                        {
                            leaderIndex = i;
                        }
                        count++;
                    }
                }
                float rotation;
                if (count == 1)
                {
                    rotation = 0f;
                }
                else
                {
                    rotation = MathHelper.Pi / (count - 1) * (index - 1) - MathHelper.PiOver2;
                }
                if (projectile.ai[0] < 0f)
                {
                    projectile.ai[0] = 0f;
                    projectile.ai[1] = 0f;
                }
                projectile.ai[0] += 0.08f;
                float time;
                if (Main.projectile[leaderIndex].ai[0] > 0f)
                {
                    if (leaderIndex == projectile.whoAmI)
                    {
                        projectile.ai[0] += 0.08f;
                    }
                    else
                    {
                        projectile.ai[0] = 0f;
                    }
                    time = Main.projectile[leaderIndex].ai[0];
                }
                else
                {
                    time = projectile.ai[0];
                    projectile.ai[0] += 0.08f;
                }
                float offX = (float)(Math.Sin(time + index * 0.6f)) * 4;
                float idleDistance = projectile.width + Main.player[projectile.owner].width + 8f * count + offX;
                var off = new Vector2(idleDistance, 0f).RotatedBy(rotation);
                off.X *= projectile.direction;
                GotoPosition(Main.player[projectile.owner].Center + off, 0.1f);

                if (projectile.rotation.Abs() < 0.1f)
                {
                    projectile.rotation = 0f;
                }
                else
                {
                    projectile.rotation = Utils.AngleLerp(projectile.rotation, 0f, 0.025f);
                }

                projectile.frameCounter++;
                if (projectile.frameCounter > 5)
                {
                    projectile.frameCounter = 0;
                    projectile.frame++;
                    if (projectile.frame > 3)
                        projectile.frame = 0;
                }
            }
            else
            {
                projectile.spriteDirection = 1;
                var differenceFromPlayer = Main.player[projectile.owner].Center - projectile.Center;
                var differenceFromNPC = Main.npc[target].Center - projectile.Center;
                var normal = Vector2.Normalize(differenceFromPlayer);

                int count = 0;
                int index = 0;
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (i == projectile.whoAmI)
                    {
                        count++;
                        index = count;
                    }
                    else if (Main.projectile[i].active && Main.projectile[i].type == projectile.type && Main.projectile[i].owner == projectile.owner)
                    {
                        count++;
                    }
                }

                projectile.ai[1] += Main.rand.NextFloat(0.3f, 2f);
                float distanceFromNPC = projectile.width + Main.npc[target].width + 10;
                if (projectile.ai[1] > 40f)
                {
                    if (projectile.ai[1] > 55f)
                    {
                        distanceFromNPC = -(projectile.ai[1] - 30f) * 4f;
                        if (projectile.ai[1] > 90f)
                        {
                            projectile.ai[1] = 0f;
                        }
                        projectile.netUpdate = true;
                    }
                    else
                    {
                        distanceFromNPC += (projectile.ai[1] - 30f) * 4f;
                    }
                }

                float rotation;
                if (count == 1)
                {
                    rotation = 0f;
                }
                else
                {
                    rotation = MathHelper.Pi / (count - 1) * (index - 1) - MathHelper.PiOver2;
                }

                GotoPosition(Main.npc[target].Center + normal.RotatedBy(rotation) * distanceFromNPC, 0.1f);
                projectile.rotation = differenceFromNPC.ToRotation();
                if (projectile.frame < 6)
                    projectile.frameCounter++;

                if (projectile.frameCounter > 9)
                {
                    if (projectile.frame <= 3)
                    {
                        projectile.frame = 4;
                        projectile.frameCounter = 5;
                    }
                    else
                    {
                        projectile.frame++;
                        projectile.frameCounter = 5;
                    }
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (projectile.ai[0] < 0f)
            {
                projectile.ai[1] = 0;
                projectile.localAI[0] = 0f;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = Main.projectileTexture[projectile.type];
            int frameHeight = texture.Height / Main.projFrames[projectile.type];
            Rectangle frame = new Rectangle(0, projectile.frame * frameHeight, texture.Width, frameHeight);
            Vector2 center = new Vector2(projectile.width / 2, projectile.height / 2);
            var effects = projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            var drawPos = projectile.position + center - Main.screenPosition;
            drawPos = new Vector2((int)drawPos.X, (int)drawPos.Y);
            Main.spriteBatch.Draw(texture, drawPos, frame, lightColor, projectile.rotation, frame.Size() / 2f, 1f, projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            return false;
        }
    }
}
