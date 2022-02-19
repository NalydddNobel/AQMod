using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.NPCs.AIs
{
    public abstract class AIFighter : ModNPC
    {
        public virtual bool WiderNPC => false;
        public virtual float SpeedCap => 2f;
        public virtual float Speed => 0.07f;
        public virtual bool KnocksOnDoors => false;
        public virtual bool OpenDoor()
        {
            npc.ai[1] += 2f;
            return npc.ai[1] > 10f;
        }

        public virtual bool JumpCheck(int tileX, int tileY)
        {
            if (npc.height >= 32 && Main.tile[tileX, tileY - 2].nactive() && Main.tileSolid[Main.tile[tileX, tileY - 2].type])
            {
                if (Main.tile[tileX, tileY - 3].nactive() && Main.tileSolid[Main.tile[tileX, tileY - 3].type])
                {
                    npc.velocity.Y = -8f;
                    npc.netUpdate = true;
                }
                else
                {
                    npc.velocity.Y = -7f;
                    npc.netUpdate = true;
                }
                return true;
            }
            else if (Main.tile[tileX, tileY - 1].nactive() && Main.tileSolid[Main.tile[tileX, tileY - 1].type])
            {
                npc.velocity.Y = -6f;
                npc.netUpdate = true;
                return true;
            }
            else if (npc.position.Y + npc.height - tileY * 16 > 20f && Main.tile[tileX, tileY].nactive() && !Main.tile[tileX, tileY].topSlope() && Main.tileSolid[Main.tile[tileX, tileY].type])
            {
                npc.velocity.Y = -5f;
                npc.netUpdate = true;
                return true;
            }
            else if (npc.directionY < 0 && (!Main.tile[tileX, tileY + 1].nactive() || !Main.tileSolid[Main.tile[tileX, tileY + 1].type]) && (!Main.tile[tileX + npc.direction, tileY + 1].nactive() || !Main.tileSolid[Main.tile[tileX + npc.direction, tileY + 1].type]))
            {
                npc.velocity.Y = -8f;
                npc.velocity.X *= 1.5f;
                npc.netUpdate = true;
                return true;
            }
            return false;
        }

        public override void AI()
        {
            int targetDelay = 60;
            bool knocksOnDoors = KnocksOnDoors;
            int npcTypeForSomeReason = npc.type;

            npc.TargetClosest(faceTarget: true);

            if (npc.velocity.X < -SpeedCap || npc.velocity.X > SpeedCap)
            {
                if (npc.velocity.Y == 0f)
                    npc.velocity *= 0.8f;
            }
            else if (npc.velocity.X < SpeedCap && npc.direction == 1)
            {
                npc.velocity.X += Speed;
                if (npc.velocity.X > SpeedCap)
                    npc.velocity.X = SpeedCap;
            }
            else if (npc.velocity.X > -SpeedCap && npc.direction == -1)
            {
                npc.velocity.X -= Speed;
                if (npc.velocity.X < -SpeedCap)
                    npc.velocity.X = -SpeedCap;
            }

            bool tileChecks = false;
            if (npc.velocity.Y == 0f)
            {
                int num77 = (int)(npc.position.Y + npc.height + 7f) / 16;
                int num189 = (int)npc.position.X / 16;
                int num79 = (int)(npc.position.X + npc.width) / 16;
                for (int num80 = num189; num80 <= num79; num80++)
                {
                    if (Main.tile[num80, num77] == null)
                        return;
                    if (Main.tile[num80, num77].nactive() && Main.tileSolid[Main.tile[num80, num77].type])
                    {
                        tileChecks = true;
                        break;
                    }
                }
            }
            if (npc.velocity.Y >= 0f)
            {
                int num81 = 0;
                if (npc.velocity.X < 0f)
                    num81 = -1;
                if (npc.velocity.X > 0f)
                    num81 = 1;
                Vector2 position3 = npc.position;
                position3.X += npc.velocity.X;
                int num82 = (int)((position3.X + npc.width / 2 + (npc.width / 2 + 1) * num81) / 16f);
                int num83 = (int)((position3.Y + npc.height - 1f) / 16f);
                if (Main.tile[num82, num83] == null)
                    Main.tile[num82, num83] = new Tile();
                if (Main.tile[num82, num83 - 1] == null)
                    Main.tile[num82, num83 - 1] = new Tile();
                if (Main.tile[num82, num83 - 2] == null)
                    Main.tile[num82, num83 - 2] = new Tile();
                if (Main.tile[num82, num83 - 3] == null)
                    Main.tile[num82, num83 - 3] = new Tile();
                if (Main.tile[num82, num83 + 1] == null)
                    Main.tile[num82, num83 + 1] = new Tile();
                if (Main.tile[num82 - num81, num83 - 3] == null)
                    Main.tile[num82 - num81, num83 - 3] = new Tile();
                if (num82 * 16 < position3.X + npc.width && num82 * 16 + 16 > position3.X && (Main.tile[num82, num83].nactive() && !Main.tile[num82, num83].topSlope() && !Main.tile[num82, num83 - 1].topSlope() && Main.tileSolid[Main.tile[num82, num83].type] && !Main.tileSolidTop[Main.tile[num82, num83].type] || Main.tile[num82, num83 - 1].halfBrick() && Main.tile[num82, num83 - 1].nactive()) && (!Main.tile[num82, num83 - 1].nactive() || !Main.tileSolid[Main.tile[num82, num83 - 1].type] || Main.tileSolidTop[Main.tile[num82, num83 - 1].type] || Main.tile[num82, num83 - 1].halfBrick() && (!Main.tile[num82, num83 - 4].nactive() || !Main.tileSolid[Main.tile[num82, num83 - 4].type] || Main.tileSolidTop[Main.tile[num82, num83 - 4].type])) && (!Main.tile[num82, num83 - 2].nactive() || !Main.tileSolid[Main.tile[num82, num83 - 2].type] || Main.tileSolidTop[Main.tile[num82, num83 - 2].type]) && (!Main.tile[num82, num83 - 3].nactive() || !Main.tileSolid[Main.tile[num82, num83 - 3].type] || Main.tileSolidTop[Main.tile[num82, num83 - 3].type]) && (!Main.tile[num82 - num81, num83 - 3].nactive() || !Main.tileSolid[Main.tile[num82 - num81, num83 - 3].type]))
                {
                    float num84 = num83 * 16;
                    if (Main.tile[num82, num83].halfBrick())
                        num84 += 8f;
                    if (Main.tile[num82, num83 - 1].halfBrick())
                        num84 -= 8f;
                    if (num84 < position3.Y + npc.height)
                    {
                        float num85 = position3.Y + npc.height - num84;
                        float num86 = 16.1f;
                        if (npc.type == NPCID.BlackRecluse || npc.type == NPCID.WallCreeper || npc.type == NPCID.JungleCreeper || npc.type == NPCID.BloodCrawler || npc.type == NPCID.DesertScorpionWalk)
                            num86 += 8f;
                        if (num85 <= num86)
                        {
                            npc.gfxOffY += npc.position.Y + npc.height - num84;
                            npc.position.Y = num84 - npc.height;
                            if (num85 < 9f)
                            {
                                npc.stepSpeed = 1f;
                            }
                            else
                            {
                                npc.stepSpeed = 2f;
                            }
                        }
                    }
                }
            }
            if (tileChecks)
            {
                int tileX = (int)((npc.position.X + npc.width / 2 + 15 * npc.direction) / 16f);
                int tileY = (int)((npc.position.Y + npc.height - 15f) / 16f);
                if (WiderNPC)
                    tileX = (int)((npc.position.X + npc.width / 2 + (npc.width / 2 + 16) * npc.direction) / 16f);
                if (Main.tile[tileX, tileY] == null)
                    Main.tile[tileX, tileY] = new Tile();
                if (Main.tile[tileX, tileY - 1] == null)
                    Main.tile[tileX, tileY - 1] = new Tile();
                if (Main.tile[tileX, tileY - 2] == null)
                    Main.tile[tileX, tileY - 2] = new Tile();
                if (Main.tile[tileX, tileY - 3] == null)
                    Main.tile[tileX, tileY - 3] = new Tile();
                if (Main.tile[tileX, tileY + 1] == null)
                    Main.tile[tileX, tileY + 1] = new Tile();
                if (Main.tile[tileX + npc.direction, tileY - 1] == null)
                    Main.tile[tileX + npc.direction, tileY - 1] = new Tile();
                if (Main.tile[tileX + npc.direction, tileY + 1] == null)
                    Main.tile[tileX + npc.direction, tileY + 1] = new Tile();
                if (Main.tile[tileX - npc.direction, tileY + 1] == null)
                    Main.tile[tileX - npc.direction, tileY + 1] = new Tile();
                Main.tile[tileX, tileY + 1].halfBrick();
                if (knocksOnDoors && Main.tile[tileX, tileY - 1].nactive() && (TileLoader.IsClosedDoor(Main.tile[tileX, tileY - 1]) || Main.tile[tileX, tileY - 1].type == 388))
                {
                    npc.ai[2] += 1f;
                    npc.ai[3] = 0f;
                    if (npc.ai[2] >= 60f)
                    {
                        npc.velocity.X = 0.5f * -npc.direction;
                        npc.ai[2] = 0f;
                        bool openDoor = OpenDoor();
                        WorldGen.KillTile(tileX, tileY - 1, fail: true);
                        if (openDoor && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            if (TileLoader.OpenDoorID(Main.tile[tileX, tileY - 1]) >= 0)
                            {
                                bool actuallyOpenedDoor = WorldGen.OpenDoor(tileX, tileY - 1, npc.direction);
                                if (!actuallyOpenedDoor)
                                {
                                    npc.ai[3] = targetDelay;
                                    npc.netUpdate = true;
                                }
                                if (Main.netMode == NetmodeID.Server && actuallyOpenedDoor)
                                    NetMessage.SendData(MessageID.ChangeDoor, -1, -1, null, 0, tileX, tileY - 1, npc.direction);
                            }
                            if (Main.tile[tileX, tileY - 1].type == 388)
                            {
                                bool flag18 = WorldGen.ShiftTallGate(tileX, tileY - 1, closing: false);
                                if (!flag18)
                                {
                                    npc.ai[3] = targetDelay;
                                    npc.netUpdate = true;
                                }
                                if (Main.netMode == NetmodeID.Server && flag18)
                                    NetMessage.SendData(MessageID.ChangeDoor, -1, -1, null, 4, tileX, tileY - 1);
                            }
                        }
                    }
                }
                else
                {
                    if (npc.velocity.X < 0f && npc.direction == -1 || npc.velocity.X > 0f && npc.direction == 1)
                    {
                        if (!JumpCheck(tileX, tileY))
                        {
                            npc.ai[1] = 0f;
                            npc.ai[2] = 0f;
                        }
                    }
                }
            }
            else if (knocksOnDoors)
            {
                npc.ai[1] = 0f;
                npc.ai[2] = 0f;
            }
        }
    }
}