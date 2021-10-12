using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.NPCs
{
    public abstract class AIMimic : ModNPC
    {
        protected virtual int GetJumpTimer() => npc.ai[1] == 0f ? 12 : 20;

        protected virtual void Jump()
        {
            if (npc.ai[1] == 2f)
            {
                npc.velocity.X = npc.direction * 2.5f;
                npc.velocity.Y = -8f;
                npc.ai[1] = 0f;
            }
            else
            {
                npc.velocity.X = npc.direction * 3.5f;
                npc.velocity.Y = -4f;
            }
        }

        public override void AI()
        {
            if (npc.ai[0] <= 0f)
            {
                npc.TargetClosest();
                Player target = Main.player[npc.target];
                if (Main.netMode == NetmodeID.MultiplayerClient)
                    return;
                if (npc.velocity.X != 0f || npc.velocity.Y < 0f || npc.velocity.Y > 0.3)
                {
                    npc.ai[0]++;
                    npc.netUpdate = true;
                    return;
                }
                var rectangle3 = new Rectangle((int)target.position.X, (int)target.position.Y, target.width, target.height);
                if (new Rectangle((int)npc.position.X - 100, (int)npc.position.Y - 100, npc.width + 200, npc.height + 200).Intersects(rectangle3) || npc.life < npc.lifeMax)
                {
                    npc.ai[0] = 1f;
                    npc.netUpdate = true;
                }
            }
            else if (npc.velocity.Y == 0f)
            {
                npc.ai[2] += 1f;
                int timer = 20;
                if (npc.ai[1] == 0f)
                    timer = 12;
                if (npc.ai[2] < timer)
                {
                    npc.velocity.X *= 0.9f;
                    return;
                }
                npc.ai[2] = 0f;
                npc.TargetClosest();
                if (npc.direction == 0)
                    npc.direction = -1;
                npc.spriteDirection = npc.direction;
                npc.ai[1] += 1f;
                Jump();
                npc.netUpdate = true;
            }
            else if (npc.direction == 1 && npc.velocity.X < 1f)
            {
                npc.velocity.X += 0.1f;
            }
            else if (npc.direction == -1 && npc.velocity.X > -1f)
            {
                npc.velocity.X -= 0.1f;
            }
        }

        public override void FindFrame(int frameHeight)
        {
            if (npc.ai[0] <= 0f)
            {
                npc.frameCounter = 0.0;
                npc.frame.Y = 0;
            }
            else
            {
                int frameCount = 3;
                if (npc.velocity.Y == 0f)
                    npc.frameCounter -= 1.0;
                else
                {
                    npc.frameCounter += 1.0;
                }
                if (npc.frameCounter < 0.0)
                    npc.frameCounter = 0.0;
                if (npc.frameCounter > frameCount * 4)
                    npc.frameCounter = frameCount * 4;
                if (npc.frameCounter < frameCount)
                    npc.frame.Y = frameHeight;
                else if (npc.frameCounter < frameCount * 2)
                {
                    npc.frame.Y = frameHeight * 2;
                }
                else if (npc.frameCounter < frameCount * 3)
                {
                    npc.frame.Y = frameHeight * 3;
                }
                else if (npc.frameCounter < frameCount * 4)
                {
                    npc.frame.Y = frameHeight * 4;
                }
                else if (npc.frameCounter < frameCount * 5)
                {
                    npc.frame.Y = frameHeight * 5;
                }
                else if (npc.frameCounter < frameCount * 6)
                {
                    npc.frame.Y = frameHeight * 4;
                }
                else if (npc.frameCounter < frameCount * 7)
                {
                    npc.frame.Y = frameHeight * 3;
                }
                else
                {
                    npc.frame.Y = frameHeight * 2;
                    if (npc.frameCounter >= frameCount * 8)
                        npc.frameCounter = frameCount;
                }
            }
        }
    }
}