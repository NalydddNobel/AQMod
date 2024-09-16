﻿using System;

namespace Aequus.Common.Entities.NPCs.AI;
public class FighterAI {
    public const float DefaultSpeedCap = 2f;
    public const float DefaultAcceleration = 0.07f;

    public static void AI(IFighterAIProvider fighter) {
        NPC NPC = fighter.NPC;
        float SpeedCap = fighter.SpeedCap;
        float Acceleration = fighter.Acceleration;

        int targetDelay = 60;
        bool knocksOnDoors = fighter.KnocksOnDoors;
        int npcTypeForSomeReason = NPC.type;

        NPC.TargetClosest(faceTarget: true);
        fighter.PostUpdateDirection();

        if (NPC.velocity.X < -SpeedCap || NPC.velocity.X > SpeedCap) {
            if (NPC.velocity.Y == 0f) {
                NPC.velocity *= 0.8f;
            }
        }
        else if (NPC.velocity.X < SpeedCap && NPC.direction == 1) {
            NPC.velocity.X += Acceleration;
            if (NPC.velocity.X > SpeedCap) {
                NPC.velocity.X = SpeedCap;
            }
        }
        else if (NPC.velocity.X > -SpeedCap && NPC.direction == -1) {
            NPC.velocity.X -= Acceleration;
            if (NPC.velocity.X < -SpeedCap) {
                NPC.velocity.X = -SpeedCap;
            }
        }

        bool tileChecks = false;
        if (NPC.velocity.Y == 0f) {
            int num77 = (int)(NPC.position.Y + NPC.height + 7f) / 16;
            int num189 = (int)NPC.position.X / 16;
            int num79 = (int)(NPC.position.X + NPC.width) / 16;
            for (int num80 = num189; num80 <= num79; num80++) {
                if (Main.tile[num80, num77] == null) {
                    return;
                }

                if (Main.tile[num80, num77].HasUnactuatedTile && Main.tileSolid[Main.tile[num80, num77].TileType]) {
                    tileChecks = true;
                    break;
                }
            }
        }
        if (NPC.velocity.Y >= 0f) {
            int direction = Math.Sign(NPC.velocity.X);

            Vector2 position3 = NPC.position;
            position3.X += NPC.velocity.X;
            int num82 = (int)((position3.X + NPC.width / 2 + (NPC.width / 2 + 1) * direction) / 16f);
            int num83 = (int)((position3.Y + NPC.height - 1f) / 16f);
            if (num82 * 16 < position3.X + NPC.width && num82 * 16 + 16 > position3.X && (Main.tile[num82, num83].HasUnactuatedTile && !Main.tile[num82, num83].TopSlope && !Main.tile[num82, num83 - 1].TopSlope && Main.tileSolid[Main.tile[num82, num83].TileType] && !Main.tileSolidTop[Main.tile[num82, num83].TileType] || Main.tile[num82, num83 - 1].IsHalfBlock && Main.tile[num82, num83 - 1].HasUnactuatedTile) && (!Main.tile[num82, num83 - 1].HasUnactuatedTile || !Main.tileSolid[Main.tile[num82, num83 - 1].TileType] || Main.tileSolidTop[Main.tile[num82, num83 - 1].TileType] || Main.tile[num82, num83 - 1].IsHalfBlock && (!Main.tile[num82, num83 - 4].HasUnactuatedTile || !Main.tileSolid[Main.tile[num82, num83 - 4].TileType] || Main.tileSolidTop[Main.tile[num82, num83 - 4].TileType])) && (!Main.tile[num82, num83 - 2].HasUnactuatedTile || !Main.tileSolid[Main.tile[num82, num83 - 2].TileType] || Main.tileSolidTop[Main.tile[num82, num83 - 2].TileType]) && (!Main.tile[num82, num83 - 3].HasUnactuatedTile || !Main.tileSolid[Main.tile[num82, num83 - 3].TileType] || Main.tileSolidTop[Main.tile[num82, num83 - 3].TileType]) && (!Main.tile[num82 - direction, num83 - 3].HasUnactuatedTile || !Main.tileSolid[Main.tile[num82 - direction, num83 - 3].TileType])) {
                float num84 = num83 * 16;
                if (Main.tile[num82, num83].IsHalfBlock) {
                    num84 += 8f;
                }

                if (Main.tile[num82, num83 - 1].IsHalfBlock) {
                    num84 -= 8f;
                }
                if (num84 < position3.Y + NPC.height) {
                    float num85 = position3.Y + NPC.height - num84;
                    float num86 = 16.1f;
                    if (NPC.type == NPCID.BlackRecluse || NPC.type == NPCID.WallCreeper || NPC.type == NPCID.JungleCreeper || NPC.type == NPCID.BloodCrawler || NPC.type == NPCID.DesertScorpionWalk) {
                        num86 += 8f;
                    }

                    if (num85 <= num86) {
                        NPC.gfxOffY += NPC.position.Y + NPC.height - num84;
                        NPC.position.Y = num84 - NPC.height;
                        if (num85 < 9f) {
                            NPC.stepSpeed = 1f;
                        }
                        else {
                            NPC.stepSpeed = 2f;
                        }
                    }
                }
            }
        }
        if (tileChecks) {
            int tileX = (int)((NPC.position.X + NPC.width / 2 + 15 * NPC.direction) / 16f);
            int tileY = (int)((NPC.position.Y + NPC.height - 15f) / 16f);

            if (fighter.IsWiderNPC) {
                tileX = (int)((NPC.position.X + NPC.width / 2 + (NPC.width / 2 + 16) * NPC.direction) / 16f);
            }

            if (knocksOnDoors && Main.tile[tileX, tileY - 1].HasUnactuatedTile && (TileLoader.IsClosedDoor(Main.tile[tileX, tileY - 1]) || Main.tile[tileX, tileY - 1].TileType == 388)) {
                NPC.ai[2] += 1f;
                NPC.ai[3] = 0f;
                if (NPC.ai[2] >= 60f) {
                    NPC.velocity.X = 0.5f * -NPC.direction;
                    NPC.ai[2] = 0f;
                    bool opensDoors = fighter.OpenDoor();
                    WorldGen.KillTile(tileX, tileY - 1, fail: true);
                    if (opensDoors && Main.netMode != NetmodeID.MultiplayerClient) {
                        if (TileLoader.OpenDoorID(Main.tile[tileX, tileY - 1]) >= 0) {
                            bool openedDoor = WorldGen.OpenDoor(tileX, tileY - 1, NPC.direction);
                            if (!openedDoor) {
                                NPC.ai[3] = targetDelay;
                                NPC.netUpdate = true;
                            }
                            if (Main.netMode == NetmodeID.Server && openedDoor) {
                                NetMessage.SendData(MessageID.ToggleDoorState, -1, -1, null, 0, tileX, tileY - 1, NPC.direction);
                            }
                        }
                        if (Main.tile[tileX, tileY - 1].TileType == 388) {
                            bool openedTallGate = WorldGen.ShiftTallGate(tileX, tileY - 1, closing: false);
                            if (!openedTallGate) {
                                NPC.ai[3] = targetDelay;
                                NPC.netUpdate = true;
                            }
                            if (Main.netMode == NetmodeID.Server && openedTallGate) {
                                NetMessage.SendData(MessageID.ToggleDoorState, -1, -1, null, 4, tileX, tileY - 1);
                            }
                        }
                    }
                }
            }
            else {
                if (NPC.velocity.X < 0f && NPC.direction == -1 || NPC.velocity.X > 0f && NPC.direction == 1) {
                    if (!fighter.JumpCheck(tileX, tileY)) {
                        NPC.ai[1] = 0f;
                        NPC.ai[2] = 0f;
                    }
                }
            }
        }
        else if (knocksOnDoors) {
            NPC.ai[1] = 0f;
            NPC.ai[2] = 0f;
        }
    }
}

public interface IFighterAIProvider {

    virtual bool IsWiderNPC => false;
    virtual float SpeedCap => FighterAI.DefaultSpeedCap;
    virtual float Acceleration => FighterAI.DefaultAcceleration;
    virtual bool KnocksOnDoors => false;

    NPC NPC { get; }

    virtual bool OpenDoor() {
        NPC.ai[1] += 2f;
        return NPC.ai[1] > 10f;
    }

    virtual bool JumpCheck(int tileX, int tileY) {
        if (NPC.height >= 32 && Main.tile[tileX, tileY - 2].HasUnactuatedTile && Main.tileSolid[Main.tile[tileX, tileY - 2].TileType]) {
            if (Main.tile[tileX, tileY - 3].HasUnactuatedTile && Main.tileSolid[Main.tile[tileX, tileY - 3].TileType]) {
                NPC.velocity.Y = -8f;
                NPC.netUpdate = true;
            }
            else {
                NPC.velocity.Y = -7f;
                NPC.netUpdate = true;
            }
            return true;
        }
        else if (Main.tile[tileX, tileY - 1].HasUnactuatedTile && Main.tileSolid[Main.tile[tileX, tileY - 1].TileType]) {
            NPC.velocity.Y = -6f;
            NPC.netUpdate = true;
            return true;
        }
        else if (NPC.position.Y + NPC.height - tileY * 16 > 20f && Main.tile[tileX, tileY].HasUnactuatedTile && !Main.tile[tileX, tileY].TopSlope && Main.tileSolid[Main.tile[tileX, tileY].TileType]) {
            NPC.velocity.Y = -5f;
            NPC.netUpdate = true;
            return true;
        }
        else if (NPC.directionY < 0 && (!Main.tile[tileX, tileY + 1].HasUnactuatedTile || !Main.tileSolid[Main.tile[tileX, tileY + 1].TileType]) && (!Main.tile[tileX + NPC.direction, tileY + 1].HasUnactuatedTile || !Main.tileSolid[Main.tile[tileX + NPC.direction, tileY + 1].TileType])) {
            NPC.velocity.Y = -8f;
            NPC.velocity.X *= 1.5f;
            NPC.netUpdate = true;
            return true;
        }
        return false;
    }

    virtual void PostUpdateDirection() {
    }
}