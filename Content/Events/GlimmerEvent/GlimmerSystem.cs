using Aequus.Common.UI.EventProgressBars;
using Aequus.Content.Events.GlimmerEvent.Peaceful;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.Enums;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Content.Events.GlimmerEvent {
    public class GlimmerSystem : ModSystem
    {
        public static int EndEventDelay;

        public override void Load()
        {
            if (!Main.dedServ)
            {
                LegacyEventProgressBarLoader.AddBar(new GlimmerProgressBar()
                {
                    EventKey = $"Mods.Aequus.Biomes.{nameof(GlimmerBiomeManager)}.DisplayName",
                    Icon = AequusTextures.Glimmer_EventIcons.Path,
                    backgroundColor = new Color(20, 75, 180, 128),
                });
            }
        }

        public static void OnTransitionToNight()
        {
            int chance = 9;
            if (Main.tenthAnniversaryWorld)
            {
                chance = 6;
            }
            if (AequusWorld.downedOmegaStarite)
            {
                chance *= 4;
            }
            if (Main.GetMoonPhase() != MoonPhase.Full && !Main.bloodMoon && NPC.AnyNPCs(NPCID.Dryad))
            {
                if (!WorldGen.spawnEye && Main.rand.NextBool(chance))
                {
                    BeginEvent();
                }
                if (!GlimmerBiomeManager.EventTechnicallyActive && Main.rand.NextBool())
                {
                    PeacefulGlimmerBiome.TileLocationX = Main.rand.Next(100, Main.maxTilesX - 100);
                }
            }
        }

        public static void DeleteFallenStarsWithin(int x)
        {
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].type == ProjectileID.FallingStar && Main.projectile[i].Center.X > x * 16f - 1000f && Main.projectile[i].Center.X < x * 16f + 1000f)
                {
                    Main.projectile[i].damage = 0;
                    Main.projectile[i].noDropItem = true;
                    Main.projectile[i].Kill();
                }
            }
        }

        public override void PreUpdatePlayers()
        {
            if (GlimmerSceneEffect.cantTouchThis > 0)
                GlimmerSceneEffect.cantTouchThis--;

            if (GlimmerBiomeManager.EventTechnicallyActive)
            {
                bool endEvent = Main.dayTime;
                if (EndEventDelay > 0)
                {
                    EndEventDelay--;
                    if (EndEventDelay <= 0)
                    {
                        endEvent = true;
                    }
                }
                PeacefulGlimmerBiome.TileLocationX = 0;
                if (endEvent)
                {
                    if (EndEvent() && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        TextHelper.Broadcast("Announcement.GlimmerEnd", TextHelper.BossSummonMessage);
                    }
                    return;
                }

                var x = GlimmerBiomeManager.TileLocation.X;
                if (GlimmerBiomeManager.TileLocation.Y == -1 || GlimmerBiomeManager.TileLocation.Y == (int)Main.worldSurface)
                {
                    GlimmerBiomeManager.TileLocation = CheckGround(GlimmerBiomeManager.TileLocation);
                }
                else if (Helper.IsSectionLoaded(GlimmerBiomeManager.TileLocation))
                {
                    if (!Main.tile[GlimmerBiomeManager.TileLocation].IsSolid())
                    {
                        GlimmerBiomeManager.TileLocation = CheckGround(GlimmerBiomeManager.TileLocation);
                    }
                }
                GlimmerBiomeManager.TileLocation = CheckGround(GlimmerBiomeManager.TileLocation);
                DeleteFallenStarsWithin(GlimmerBiomeManager.TileLocation.X);
                if (Main.netMode == NetmodeID.Server && GlimmerBiomeManager.TileLocation.X != x)
                {
                    SendGlimmerStatus();
                }
            }
            else
            {
                EndEventDelay = 0;
            }
            if (PeacefulGlimmerBiome.EventActive)
            {
                DeleteFallenStarsWithin(PeacefulGlimmerBiome.TileLocationX);
                if (Main.dayTime || Main.bloodMoon || Main.snowMoon || Main.pumpkinMoon)
                {
                    PeacefulGlimmerBiome.TileLocationX = 0;
                }
            }
        }

        public override void PostUpdateEverything()
        {
            if (GlimmerBiomeManager.omegaStarite != -1 && (!Main.npc[GlimmerBiomeManager.omegaStarite].active || !Main.npc[GlimmerBiomeManager.omegaStarite].boss))
            {
                GlimmerBiomeManager.omegaStarite = -1;
            }
        }

        public static void BeginEvent(Point where)
        {
            GlimmerBiomeManager.TileLocation = CheckGround(where);

            TextHelper.Broadcast($"Announcement.GlimmerStart{(where.X * 2 > Main.maxTilesX ? "East" : "West")}", TextHelper.BossSummonMessage);
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                SendGlimmerStatus();
            }
        }

        public static bool BeginEvent()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                Aequus.GetPacket(PacketType.RequestGlimmerEvent).Send();
                return false;
            }

            for (int i = 0; i < 1000; i++)
            {
                int x = Main.rand.Next(200, Main.maxTilesX - 200);
                if ((x - Main.spawnTileX).Abs() < GlimmerBiomeManager.SuperStariteTile)
                    continue;
                if (i < 100)
                {
                    bool nearBed = false;
                    for (int j = 0; j < Main.maxPlayers; j++)
                    {
                        if (Main.player[j].active && !Main.player[j].dead && Main.player[j].GetSpawnY() <= Main.worldSurface)
                        {
                            //AequusText.Broadcast($"{j}/{(x - Main.player[j].GetSpawnX()).Abs()}", Color.Red);
                            if ((x - Main.player[j].GetSpawnX()).Abs() <= GlimmerBiomeManager.HyperStariteTile)
                            {
                                nearBed = true;
                                break;
                            }
                        }
                    }
                    if (nearBed)
                        continue;
                }
                //AequusText.Broadcast((x - Main.spawnTileX).Abs().ToString(), Main.DiscoColor);
                BeginEvent(new Point(x, -1));
                return true;
            }
            return false;
        }

        public static bool EndEvent()
        {
            if (!GlimmerBiomeManager.EventTechnicallyActive)
            {
                return false;
            }

            EndEventDelay = 0;
            GlimmerBiomeManager.TileLocation = Point.Zero;
            if (Main.netMode == NetmodeID.Server)
            {
                SendGlimmerStatus();
            }
            return true;
        }

        public static int CalcTiles(Player player)
        {
            return (int)((player.position.X + player.width) / 16 - GlimmerBiomeManager.TileLocation.X).Abs();
        }

        public static Point CheckGround(Point p)
        {
            ushort min = (ushort)(90 * (Main.maxTilesY / (AequusWorld.SmallHeight / 2)));
            min -= 50;
            p.Y = Math.Max(p.Y, min);

            for (ushort j = min; j <= Main.worldSurface; j++)
            {
                if (!Helper.IsSectionLoaded(p.X, j))
                    continue;

                if (Main.tile[p.X, j].IsSolid())
                {
                    p.Y = j;
                    break;
                }
            }
            if (p.Y != (int)Main.worldSurface)
            {
                for (ushort j = (ushort)p.Y; j > min; j--)
                {
                    for (ushort k = 0; k < 10; k++)
                    {
                        if (!Helper.IsSectionLoaded(p.X, j - k))
                            continue;

                        if (Main.tile[p.X, j - k].IsSolid())
                        {
                            goto FoundInvalidSpot;
                        }
                    }
                    p.Y = j + 1;
                    return p;

                FoundInvalidSpot:
                    continue;
                }
                return p;
            }
            p.Y = (ushort)Main.worldSurface;
            return p;
        }

        public static void ResetWorldData()
        {
            GlimmerBiomeManager.TileLocation = Point.Zero;
            PeacefulGlimmerBiome.TileLocationX = 0;
            EndEventDelay = 0;
        }

        public override void OnWorldLoad()
        {
            ResetWorldData();
        }

        public override void OnWorldUnload()
        {
            ResetWorldData();
        }

        public override void SaveWorldData(TagCompound tag)
        {
            if (PeacefulGlimmerBiome.EventActive)
            {
                tag["PeacefulGlimmerX"] = PeacefulGlimmerBiome.TileLocationX;
            }
            if (!GlimmerBiomeManager.EventActive)
            {
                return;
            }
            tag["GlimmerX"] = GlimmerBiomeManager.TileLocation.X;
            tag["GlimmerY"] = GlimmerBiomeManager.TileLocation.Y;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            if (tag.TryGet("PeacefulGlimmerX", out int peacefulX))
                PeacefulGlimmerBiome.TileLocationX = peacefulX;
            if (tag.TryGet("GlimmerX", out int x) && tag.TryGet("GlimmerY", out int y))
                GlimmerBiomeManager.TileLocation = new Point(x, y);
        }

        public override void NetSend(BinaryWriter writer)
        {
            var bb = new BitsByte(GlimmerBiomeManager.EventActive, PeacefulGlimmerBiome.EventActive);
            writer.Write(bb);
            if (bb[0])
            {
                writer.Write((ushort)GlimmerBiomeManager.TileLocation.X);
                writer.Write((ushort)GlimmerBiomeManager.TileLocation.Y);
            }
            if (bb[1])
            {
                writer.Write((ushort)PeacefulGlimmerBiome.TileLocationX);
            }
        }

        public override void NetReceive(BinaryReader reader)
        {
            var bb = (BitsByte)reader.ReadByte();
            if (bb[0])
            {
                GlimmerBiomeManager.TileLocation = new Point(reader.ReadUInt16(), reader.ReadUInt16());
            }
            if (bb[1])
            {
                PeacefulGlimmerBiome.TileLocationX = reader.ReadUInt16();
            }
        }

        public override void PostUpdateTime()
        {
            if (!CreativePowerManager.Instance.GetPower<CreativePowers.FreezeTime>().Enabled && GlimmerBiomeManager.EventActive)
            {
                int playersInTimeWound = 0;
                int maxPlayers = 0;
                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    if (Main.player[i].active && !Main.player[i].dead)
                    {
                        maxPlayers++;
                        if (Main.player[i].Distance(GlimmerBiomeManager.TileLocation.ToWorldCoordinates()) < 250f * 16f)
                        {
                            playersInTimeWound++;
                        }
                    }
                }
                if (maxPlayers > 0 && playersInTimeWound / (float)maxPlayers > 0.5f)
                {
                    Main.time -= 2.0d * CreativePowerManager.Instance.GetPower<CreativePowers.ModifyTimeRate>().TargetTimeRate;
                    if (Main.time <= 1.0d)
                    {
                        Main.time = 1.0d;
                    }
                }
            }
        }

        public static void SendGlimmerStatus()
        {
            PacketSystem.Send((p) =>
            {
                p.Write(GlimmerBiomeManager.EventActive);
                if (GlimmerBiomeManager.EventActive)
                {
                    p.Write((ushort)GlimmerBiomeManager.TileLocation.X);
                    p.Write((ushort)GlimmerBiomeManager.TileLocation.Y);
                }
            }, PacketType.GlimmerStatus);
        }

        public static void ReadGlimmerStatus(BinaryReader r)
        {
            if (r.ReadBoolean())
            {
                GlimmerBiomeManager.TileLocation = new Point(r.ReadUInt16(), r.ReadUInt16());
                return;
            }
            GlimmerBiomeManager.TileLocation = Point.Zero;
        }
    }
}