using Aequus.Biomes;
using Aequus.Biomes.DemonSiege;
using Aequus.Biomes.Glimmer;
using Aequus.Content;
using Aequus.Content.CarpenterBounties;
using Aequus.Content.DronePylons;
using Aequus.Content.Necromancy;
using Aequus.NPCs.Boss;
using Aequus.Projectiles.Summon;
using Aequus.Tiles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Common.Networking
{
    public sealed class PacketHandler : ModSystem
    {
        private static HashSet<PacketType> logPacketType;

        public static ModPacket NewPacket => Aequus.Instance.GetPacket();

        public override void Load()
        {
            logPacketType = new HashSet<PacketType>()
            {
                PacketType.GiveoutEnemySouls,
                PacketType.GlimmerEventUpdate,
                PacketType.RemoveDemonSiege,
                PacketType.SetExporterQuestsCompleted,
                PacketType.SpawnOmegaStarite,
                PacketType.StartDemonSiege,
                PacketType.SyncDronePoint,
            };
        }

        public static void Send(PacketType type, int capacity = 256, int to = -1, int ignore = -1)
        {
            var packet = Aequus.Instance.GetPacket(capacity);
            packet.Write((byte)type);
        }

        public static void Send(Func<ModPacket, bool> func, PacketType type, int capacity = 256, int to = -1, int ignore = -1)
        {
            var packet = Aequus.Instance.GetPacket(capacity);
            packet.Write((byte)type);
            if (func(packet))
                packet.Send(to, ignore);
        }

        public static void Send(Action<ModPacket> action, PacketType type, int capacity = 256, int to = -1, int ignore = -1)
        {
            var packet = Aequus.Instance.GetPacket(capacity);
            packet.Write((byte)type);
            action(packet);
            packet.Send(to, ignore);
        }

        public static void SendSound(string name, Vector2? location = null, float? volume = null, float? pitch = null)
        {
            Send((p) =>
            {
                p.Write(name);
                FlaggedWrite(location != null, (p) => p.WriteVector2(location.Value), p);
                FlaggedWrite(volume != null, (p) => p.Write(volume.Value), p);
                FlaggedWrite(pitch != null, (p) => p.Write(pitch.Value), p);
            }, PacketType.SoundQueue);
        }

        public static void FlaggedWrite(bool flag, Action<ModPacket> writeAction, ModPacket p)
        {
            p.Write(flag);
            if (flag)
            {
                writeAction(p);
            }
        }

        public static void SyncNecromancyOwner(int npc, int player)
        {
            Send((p) =>
                {
                    p.Write(npc);
                    p.Write(player);
                },
                PacketType.SyncNecromancyOwner);
        }

        public static void WriteNullableItem(Item item, BinaryWriter writer, bool writeStack = false, bool writeFavorite = false)
        {
            if (item != null)
            {
                writer.Write(true);
                ItemIO.Send(item, writer, writeStack, writeFavorite);
            }
            else
            {
                writer.Write(false);
            }
        }
        public static Item ReadNullableItem(BinaryReader reader, bool readStack = false, bool readFavorite = false)
        {
            if (reader.ReadBoolean())
            {
                var item = new Item();
                ItemIO.Receive(item, reader, readStack, readFavorite);
                return item;
            }
            else
            {
                return null;
            }
        }

        public static void WriteNullableItemList(Item[] items, BinaryWriter writer, bool writeStack = false, bool writeFavorite = false)
        {
            if (items != null)
            {
                writer.Write(true);
                if (items.Length < 0 || items.Length > byte.MaxValue)
                {
                    throw new Exception("Length of item list is invalid, must not go below 0 nor be greater than 255");
                }
                writer.Write((byte)items.Length);
                for (int i = 0; i < items.Length; i++)
                {
                    WriteNullableItem(items[i], writer, writeStack, writeFavorite);
                }
            }
            else
            {
                writer.Write(false);
            }
        }
        public static Item[] ReadNullableItemList(BinaryReader reader, bool readStack = false, bool readFavorite = false)
        {
            if (reader.ReadBoolean())
            {
                var item = new Item[reader.ReadByte()];
                for (int i = 0; i < item.Length; i++)
                {
                    item[i] = ReadNullableItem(reader, readStack, readFavorite);
                }
                return item;
            }
            else
            {
                return null;
            }
        }

        public static PacketType ReadPacketType(BinaryReader reader)
        {
            return (PacketType)reader.ReadByte();
        }

        public static void SyncNPC(NPC npc)
        {
            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
        }

        public static void HandlePacket(BinaryReader reader)
        {
            var type = ReadPacketType(reader);

            var l = Aequus.Instance.Logger;
            if (logPacketType.Contains(type))
            {
                l.Debug("Recieving Packet: " + type);
            }
            switch (type)
            {
                case PacketType.CarpenterBountiesCompleted:
                    {
                        int player = reader.ReadInt32();
                        Main.player[player].GetModPlayer<CarpenterBountyPlayer>().RecieveClientChanges(reader);
                    }
                    break;

                case PacketType.RequestTileSectionFromServer:
                    {
                        int plr = reader.ReadInt32();
                        int sectionX = reader.ReadInt32();
                        int sectionY = reader.ReadInt32();
                        if (Main.netMode == NetmodeID.Server)
                        {
                            NetMessage.SendSection(plr, sectionX, sectionY);
                        }
                    }
                    break;

                case PacketType.SyncDronePoint:
                    {
                        int x = reader.ReadInt32();
                        int y = reader.ReadInt32();

                        DroneWorld.RecievePacket(reader, new Point(x, y));
                    }
                    break;

                case PacketType.SyncNecromancyNPC:
                    {
                        byte npc = reader.ReadByte();
                        Main.npc[npc].GetGlobalNPC<NecromancyNPC>().Receive(reader);
                    }
                    break;

                case PacketType.SpawnOmegaStarite:
                    AequusText.Broadcast("Announcement.GlimmerStart", GlimmerBiome.TextColor);
                    NPC.SpawnBoss(reader.ReadInt32(), reader.ReadInt32() - 1600, ModContent.NPCType<OmegaStarite>(), reader.ReadInt32());
                    break;

                case PacketType.SetExporterQuestsCompleted:
                    ExporterQuests.QuestsCompleted = reader.ReadUInt16();
                    break;

                case PacketType.GlimmerEventUpdate:
                    GlimmerSystem.RecieveGlimmerEventUpdate(reader);
                    break;

                case PacketType.GiveoutEnemySouls:
                    {
                        int amt = reader.ReadInt32();
                        var v = reader.ReadVector2();
                        for (int i = 0; i < amt; i++)
                        {
                            int player = reader.ReadInt32();
                            if (Main.myPlayer == player)
                                Projectile.NewProjectile(new EntitySource_Sync("PacketType.GiveoutEnemySouls"), v, Main.rand.NextVector2Unit() * 1.5f, ModContent.ProjectileType<SoulAbsorbtion>(), 0, 0f, player);
                            Main.player[player].GetModPlayer<AequusPlayer>().candleSouls++;
                        }
                    }
                    break;

                case PacketType.SyncRecyclingMachine:
                    TERecyclingMachine.NetReceive2(reader);
                    break;

                case PacketType.SyncAequusNPC:
                    {
                        byte npc = reader.ReadByte();
                        Main.npc[npc].Aequus().Receive(npc, reader);
                    }
                    break;

                case PacketType.RemoveDemonSiege:
                    DemonSiegeSystem.ActiveSacrifices.Remove(new Point(reader.ReadUInt16(), reader.ReadUInt16()));
                    break;

                case PacketType.StartDemonSiege:
                    DemonSiegeSystem.ReceiveStartRequest(reader);
                    break;

                case PacketType.DemonSiegeSacrificeStatus:
                    DemonSiegeSacrifice.ReceiveStatus(reader);
                    break;

                case PacketType.SoundQueue:
                    SoundHelpers.ReceiveSoundQueue(reader);
                    break;

                case PacketType.SyncAequusPlayer:
                    {
                        if (Main.player[reader.ReadByte()].TryGetModPlayer<AequusPlayer>(out var aequus))
                        {
                            aequus.RecieveChanges(reader);
                        }
                    }
                    break;

                case PacketType.SyncNecromancyOwner:
                    {
                        int npc = reader.ReadInt32();
                        Main.npc[npc].GetGlobalNPC<NecromancyNPC>().zombieOwner = reader.ReadInt32();
                    }
                    break;

                default:
                    break;
            }
        }
    }
}