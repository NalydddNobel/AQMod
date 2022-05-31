using Aequus.Biomes;
using Aequus.Content.Necromancy;
using Aequus.NPCs;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Common.Networking
{
    public sealed class PacketHandler : ModSystem
    {
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

        public static void SyncNecromancyOwnerTier(int npc, int player, float tier)
        {
            Send((p) =>
                {
                    p.Write(npc);
                    p.Write(player);
                    p.Write(tier);
                },
                PacketType.SyncNecromancyOwnerTier);
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

        public static void HandlePacket(BinaryReader reader)
        {
            PacketType type = ReadPacketType(reader);

            var l = Aequus.Instance.Logger;
            if (type != PacketType.Unused && type != PacketType.SyncAequusPlayer)
            {
                l.Debug("Recieving Packet: " + type);
            }
            if (type == PacketType.Unused)
            {
            }
            else if (type == PacketType.SyncNecromancyOwnerTier)
            {
                int npc = reader.ReadInt32();
                Main.npc[npc].GetGlobalNPC<NecromancyNPC>().zombieOwner = reader.ReadInt32();
                Main.npc[npc].GetGlobalNPC<NecromancyNPC>().zombieDebuffTier = reader.ReadSingle();
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
        }
    }
}