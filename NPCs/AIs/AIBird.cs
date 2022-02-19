using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.NPCs.AIs
{
    public abstract class AIBird : ModNPC
    {
        public override void AI()
        {
            npc.noGravity = true;
            if (npc.ai[0] == 0f)
            {
                npc.noGravity = false;
                npc.TargetClosest(faceTarget: true);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (npc.velocity.X != 0f || npc.velocity.Y < 0f || (double)npc.velocity.Y > 0.3)
                    {
                        npc.ai[0] = 1f;
                        npc.netUpdate = true;
                        npc.direction = -npc.direction;
                    }
                    else
                    {
                        Rectangle rectangle2 = new Rectangle((int)Main.player[npc.target].position.X, (int)Main.player[npc.target].position.Y, Main.player[npc.target].width, Main.player[npc.target].height);
                        if (new Rectangle((int)npc.position.X - 100, (int)npc.position.Y - 100, npc.width + 200, npc.height + 200).Intersects(rectangle2) || npc.life < npc.lifeMax)
                        {
                            npc.ai[0] = 1f;
                            npc.velocity.Y -= 6f;
                            npc.netUpdate = true;
                            npc.direction = -npc.direction;
                        }
                    }
                }
            }
            else if (!Main.player[npc.target].dead)
            {
                if (npc.collideX)
                {
                    npc.direction *= -1;
                    npc.velocity.X = npc.oldVelocity.X * -0.5f;
                    if (npc.direction == -1 && npc.velocity.X > 0f && npc.velocity.X < 2f)
                    {
                        npc.velocity.X = 2f;
                    }
                    if (npc.direction == 1 && npc.velocity.X < 0f && npc.velocity.X > -2f)
                    {
                        npc.velocity.X = -2f;
                    }
                }
                if (npc.collideY)
                {
                    npc.velocity.Y = npc.oldVelocity.Y * -0.5f;
                    if (npc.velocity.Y > 0f && npc.velocity.Y < 1f)
                    {
                        npc.velocity.Y = 1f;
                    }
                    if (npc.velocity.Y < 0f && npc.velocity.Y > -1f)
                    {
                        npc.velocity.Y = -1f;
                    }
                }
                if (npc.direction == -1 && npc.velocity.X > -3f)
                {
                    npc.velocity.X -= 0.1f;
                    if (npc.velocity.X > 3f)
                    {
                        npc.velocity.X -= 0.1f;
                    }
                    else if (npc.velocity.X > 0f)
                    {
                        npc.velocity.X -= 0.05f;
                    }
                    if (npc.velocity.X < -3f)
                    {
                        npc.velocity.X = -3f;
                    }
                }
                else if (npc.direction == 1 && npc.velocity.X < 3f)
                {
                    npc.velocity.X += 0.1f;
                    if (npc.velocity.X < -3f)
                    {
                        npc.velocity.X += 0.1f;
                    }
                    else if (npc.velocity.X < 0f)
                    {
                        npc.velocity.X += 0.05f;
                    }
                    if (npc.velocity.X > 3f)
                    {
                        npc.velocity.X = 3f;
                    }
                }
                int num830 = (int)((npc.position.X + (float)(npc.width / 2)) / 16f) + npc.direction;
                int num831 = (int)((npc.position.Y + (float)npc.height) / 16f);
                bool flag29 = true;
                int num832 = 15;
                bool flag30 = false;
                for (int num834 = num831; num834 < num831 + num832; num834++)
                {
                    if (Main.tile[num830, num834] == null)
                    {
                        Main.tile[num830, num834] = new Tile();
                    }
                    if ((Main.tile[num830, num834].nactive() && Main.tileSolid[Main.tile[num830, num834].type]) || Main.tile[num830, num834].liquid > 0)
                    {
                        if (num834 < num831 + 5)
                        {
                            flag30 = true;
                        }
                        flag29 = false;
                        break;
                    }
                }
                if (flag29)
                {
                    npc.velocity.Y += 0.05f;
                }
                else
                {
                    npc.velocity.Y -= 0.1f;
                }
                if (flag30)
                {
                    npc.velocity.Y -= 0.2f;
                }
                if (npc.velocity.Y > 2f)
                {
                    npc.velocity.Y = 2f;
                }
                if (npc.velocity.Y < -4f)
                {
                    npc.velocity.Y = -4f;
                }
            }
            if (npc.wet)
            {
                npc.ai[1] = 0f;
                if (npc.velocity.Y > 0f)
                {
                    npc.velocity.Y *= 0.95f;
                }
                npc.velocity.Y -= 0.5f;
                if (npc.velocity.Y < -4f)
                {
                    npc.velocity.Y = -4f;
                }
                npc.TargetClosest();
            }
        }
    }
}