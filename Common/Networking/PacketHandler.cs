using Aequus.Biomes;
using Aequus.Content;
using Aequus.Content.Necromancy;
using Aequus.NPCs;
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
                [PacketType.SetExporterQuestsCompleted] = new Procedure
                ((p, o) =>
                {
                    p.Write((ushort)ExporterQuests.QuestsCompleted);
                },
                (r) =>
                {
                    ExporterQuests.QuestsCompleted = r.ReadUInt16();
                }),
            };
        }

        public static void SendProcedure(PacketType packetType, params object[] obj)
        {
            Send((p) =>
            {
                procedures[packetType].Send(p, obj);
            }, packetType);
        }

        public static void ReadProcedure(PacketType packetType, BinaryReader reader)
        {
            if (procedures.TryGetValue(packetType, out var p))
            {
                p.Read(reader);
            }
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

        public static void SyncNecromancyOwner(int npc, int player, float tier)
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
            if (type != PacketType.Unused && type != PacketType.SyncAequusPlayer)
            {
                l.Debug("Recieving Packet: " + type);
            }
            if (type == PacketType.Unused)
            {
            }
            else if (type == PacketType.SyncNecromancyOwner)
            {
                int npc = reader.ReadInt32();
                Main.npc[npc].GetGlobalNPC<NecromancyNPC>().zombieOwner = reader.ReadInt32();
            }
            else if (type == PacketType.SyncAequusPlayer)
            {
                if (Main.player[reader.ReadByte()].TryGetModPlayer<AequusPlayer>(out var aequus))
                {
                    aequus.RecieveChanges(reader);
                }
            }
            else if (type == PacketType.SoundQueue)
            {
                SoundHelpers.ReadSoundQueue(reader);
            }
            else if (type == PacketType.DemonSiegeSacrificeStatus)
            {
                DemonSiegeInvasion.EventSacrifice.ReadPacket(reader);
            }
            else if (type == PacketType.RequestDemonSiege)
            {
                DemonSiegeInvasion.HandleStartRequest(reader);
            }
            else if (type == PacketType.RemoveDemonSiege)
            {
                DemonSiegeInvasion.Sacrifices.Remove(new Point(reader.ReadUInt16(), reader.ReadUInt16()));
            }
            else if (type == PacketType.SyncDebuffs)
            {
                byte npc = reader.ReadByte();
                Main.npc[npc].GetGlobalNPC<NPCDebuffs>().Receive(npc, reader);
            }
            else if (type == PacketType.SyncRecyclingMachine_CauseForSomeReasonNetRecieveIsntWorkingOnTileEntities)
            {
                TERecyclingMachine.NetReceive2(reader);
            }
            else if (type == PacketType.GiveoutEnemySouls)
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
            else
            {
                ReadProcedure(type, reader);
            }
        }
    }
}