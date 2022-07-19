using Aequus.Biomes.DemonSiege;
using Aequus.Biomes.Glimmer;
using Aequus.Content;
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
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Common.Networking
{
    public sealed class PacketHandler : ModSystem
    {
        public delegate void SendProcedureMethod(ModPacket packet, object[] args);
        public delegate void ReadProcedureMethod(BinaryReader reader);
        public struct Procedure
        {
            public SendProcedureMethod Send;
            public ReadProcedureMethod Read;

            public Procedure(SendProcedureMethod send, ReadProcedureMethod read)
            {
                Send = send;
                Read = read;
            }
        }

        private static Dictionary<PacketType, Procedure> procedures;

        public override void Load()
        {
            procedures = new Dictionary<PacketType, Procedure>()
            {
                [PacketType.SpawnOmegaStarite] = new Procedure
                ((p, o) =>
                {
                    if (o[0] != null && o[0] is string)
                    {
                        p.Write((string)o[0]);
                    }
                    else
                    {
                        p.Write("");
                    }
                    p.Write((int)o[1]);
                    p.WriteVector2((Vector2)o[2]);
                    var b = new BitsByte();

                    b[0] = o.Length > 3 && o[3] is float && (float)o[3] == 0f;
                    b[1] = o.Length > 4 && o[4] is float && (float)o[4] == 0f;
                    b[2] = o.Length > 5 && o[5] is float && (float)o[5] == 0f;
                    b[3] = o.Length > 6 && o[6] is float && (float)o[6] == 0f;

                    p.Write(b);

                    if (b[0])
                    {
                        p.Write((float)o[3]);
                    }
                    if (b[1])
                    {
                        p.Write((float)o[4]);
                    }
                    if (b[2])
                    {
                        p.Write((float)o[5]);
                    }
                    if (b[3])
                    {
                        p.Write((float)o[6]);
                    }
                },
                (r) =>
                {
                    float[] ai = new float[4];
                    string syncSource = r.ReadString();
                    int netID = r.ReadInt32();
                    var location = r.ReadVector2();
                    var b = (BitsByte)r.ReadByte();
                    if (b[0])
                    {
                        ai[0] = r.ReadSingle();
                    }
                    if (b[1])
                    {
                        ai[1] = r.ReadSingle();
                    }
                    if (b[2])
                    {
                        ai[2] = r.ReadSingle();
                    }
                    if (b[3])
                    {
                        ai[3] = r.ReadSingle();
                    }
                    NPC.NewNPCDirect(new EntitySource_Sync(syncSource), location, netID, 0, ai[0], ai[1], ai[2], ai[3]);
                }),
            };
        }

        public static void SendLegacyProcedure(PacketType packetType, params object[] obj)
        {
            Send((p) =>
            {
                procedures[packetType].Send(p, obj);
            }, packetType);
        }

        public static void ReadLegacyProcedure(PacketType packetType, BinaryReader reader)
        {
            if (procedures.TryGetValue(packetType, out var p))
            {
                p.Read(reader);
            }
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
                FlaggedSend(location != null, (p) => p.WriteVector2(location.Value), p);
                FlaggedSend(volume != null, (p) => p.Write(volume.Value), p);
                FlaggedSend(pitch != null, (p) => p.Write(pitch.Value), p);
            }, PacketType.SoundQueue);
        }

        public static void FlaggedSend(bool flag, Action<ModPacket> writeAction, ModPacket p)
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
            if (type != PacketType.SyncAequusPlayer)
            {
                l.Debug("Recieving Packet: " + type);
            }
            switch (type)
            {
                case PacketType.SyncNecromancyNPC:
                    {
                        byte npc = reader.ReadByte();
                        Main.npc[npc].GetGlobalNPC<NecromancyNPC>().Receive(reader);
                    }
                    break;

                case PacketType.SpawnOmegaStarite:
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
                    ReadLegacyProcedure(type, reader);
                    break;
            }
        }
    }
}