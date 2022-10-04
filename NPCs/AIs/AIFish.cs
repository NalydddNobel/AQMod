using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs.AIs
{
    public abstract class AIFish : ModNPC
    {
        public virtual bool FlopsOutOfWater => true;

        public virtual bool ValidTarget(Player player)
        {
            return player.wet && !player.dead && Collision.CanHit(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height);

        }

        public virtual void LandMovement()
        {
            if (NPC.velocity.Y == 0f)
            {
                if (!FlopsOutOfWater)
                {
                    NPC.velocity.X *= 0.94f;
                    if (NPC.velocity.X > -0.2 && NPC.velocity.X < 0.2)
                    {
                        NPC.velocity.X = 0f;
                    }
                }
                else if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.velocity.Y = Main.rand.Next(-50, -20) * 0.1f;
                    NPC.velocity.X = Main.rand.Next(-20, 20) * 0.1f;
                    NPC.netUpdate = true;
                }
            }
            NPC.velocity.Y += 0.3f;
            if (NPC.velocity.Y > 10f)
            {
                NPC.velocity.Y = 10f;
            }
            NPC.ai[0] = 1f;
        }

        public virtual void GetIdleSpeeds(out float speedX, out float speedY, out Vector2 capX, out float capY)
        {
            speedX = 0.1f;
            speedY = 0.01f;
            capX = new Vector2(1f, 0.95f);
            capY = 0.3f;
        }

        public virtual void WaterIdleMovement()
        {
            GetIdleSpeeds(out float speedX, out float speedY, out var capX, out float capY);
            NPC.velocity.X += NPC.direction * 0.1f;
            if (NPC.velocity.X < -capX.X || NPC.velocity.X > capX.X)
            {
                NPC.velocity.X *= capX.Y;
            }
            if (NPC.ai[0] == -1f)
            {
                NPC.velocity.Y -= speedY;
                if (NPC.velocity.Y < -capY)
                {
                    NPC.ai[0] = 1f;
                }
            }
            else
            {
                NPC.velocity.Y += speedY;
                if (NPC.velocity.Y > capY)
                {
                    NPC.ai[0] = -1f;
                }
            }
        }

        public virtual void WaterIdleMovement_WallChecks()
        {
            int tileX = (int)(NPC.position.X + (float)(NPC.width / 2)) / 16;
            int tileY = (int)(NPC.position.Y + (float)(NPC.height / 2)) / 16;
            if (Main.tile[tileX, tileY - 1].LiquidAmount > 128)
            {
                if (Main.tile[tileX, tileY + 1].HasTile)
                {
                    NPC.ai[0] = -1f;
                }
                else if (Main.tile[tileX, tileY + 2].HasTile)
                {
                    NPC.ai[0] = -1f;
                }
            }
        }

        public virtual void WaterIdleMovement_ApplyVelocityYCap()
        {
            if (NPC.velocity.Y > 0.4 || NPC.velocity.Y < -0.4)
            {
                NPC.velocity.Y *= 0.95f;
            }
        }

        public virtual void WaterAdvancedWallChecks()
        {
            int tileX = (int)NPC.Center.X / 16;
            int tileY = (int)(NPC.position.Y + NPC.height) / 16;
            if (Main.tile[tileX, tileY].TopSlope)
            {
                if (Main.tile[tileX, tileY].LeftSlope)
                {
                    NPC.direction = -1;
                    NPC.velocity.X = Math.Abs(NPC.velocity.X) * -1f;
                }
                else
                {
                    NPC.direction = 1;
                    NPC.velocity.X = Math.Abs(NPC.velocity.X);
                }
            }
            else if (Main.tile[tileX, tileY + 1].TopSlope)
            {
                if (Main.tile[tileX, tileY + 1].LeftSlope)
                {
                    NPC.direction = -1;
                    NPC.velocity.X = Math.Abs(NPC.velocity.X) * -1f;
                }
                else
                {
                    NPC.direction = 1;
                    NPC.velocity.X = Math.Abs(NPC.velocity.X);
                }
            }
        }

        public virtual void WaterIdleMovement_BumpIntoWalls()
        {
            if (NPC.collideX)
            {
                NPC.velocity.X *= -1f;
                NPC.direction *= -1;
                NPC.netUpdate = true;
            }
            if (NPC.collideY)
            {
                NPC.netUpdate = true;
                if (NPC.velocity.Y > 0f)
                {
                    NPC.velocity.Y = Math.Abs(NPC.velocity.Y) * -1f;
                    NPC.directionY = -1;
                    NPC.ai[0] = -1f;
                }
                else if (NPC.velocity.Y < 0f)
                {
                    NPC.velocity.Y = Math.Abs(NPC.velocity.Y);
                    NPC.directionY = 1;
                    NPC.ai[0] = 1f;
                }
            }
        }

        public virtual void GetChaseSpeeds(out float speedX, out float speedY, out Vector2 capX, out Vector2 capY)
        {
            speedX = 0.1f;
            speedY = 0.1f;
            capX = new Vector2(3f);
            capY = new Vector2(2f);
        }

        public virtual void WaterChaseMovement()
        {
            GetChaseSpeeds(out float speedX, out float speedY, out var capX, out var capY);
            NPC.velocity.X += NPC.direction * speedX;
            NPC.velocity.Y += NPC.directionY * speedY;
            if (NPC.velocity.X > capX.X)
            {
                NPC.velocity.X = capX.Y;
            }
            if (NPC.velocity.X < -capX.X)
            {
                NPC.velocity.X = -capX.Y;
            }
            if (NPC.velocity.Y > capY.X)
            {
                NPC.velocity.Y = capY.Y;
            }
            if (NPC.velocity.Y < -capY.X)
            {
                NPC.velocity.Y = -capY.Y;
            }
        }

        public override void AI()
        {
            if (NPC.direction == 0)
            {
                NPC.TargetClosest();
            }

            if (NPC.wet)
            {
                bool hasTarget = false;
                if (!NPC.friendly)
                {
                    NPC.TargetClosest(faceTarget: false);
                    hasTarget = ValidTarget(Main.player[NPC.target]);
                }

                WaterAdvancedWallChecks();
                if (hasTarget)
                {
                    NPC.TargetClosest();
                    WaterChaseMovement();
                }
                else
                {
                    WaterIdleMovement_BumpIntoWalls();
                    WaterIdleMovement();

                    WaterIdleMovement_WallChecks();
                    WaterIdleMovement_ApplyVelocityYCap();
                }
            }
            else
            {
                LandMovement();
            }
            NPC.rotation = Math.Clamp(NPC.velocity.Y * NPC.direction * 0.1f, -0.2f, 0.2f);
        }
    }
}
