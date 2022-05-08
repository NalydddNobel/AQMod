using Aequus.NPCs;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Common.Networking
{
    public sealed class PacketSender : ModSystem
    {
        public override bool HijackSendData(int whoAmI, int msgType, int remoteClient, int ignoreClient, NetworkText text, int number, float number2, float number3, float number4, int number5, int number6, int number7)
        {
            if (msgType == MessageID.SyncNPC)
            {
                try
                {
                    SendNPCNetworkerGlobals(remoteClient, ignoreClient, number);
                }
                catch
                {
                }
            }
            return false;
        }
        public static void SendNPCNetworkerGlobals(int remoteClient, int ignoreClient, int npc)
        {
            Send((p) =>
            {
                p.Write(npc);
                var globals = GetNetworkerGlobals(Main.npc[npc]);
                for (int i = 0; i < globals.Length; i++)
                {
                    globals[i].Send(p);
                }
            },
            PacketType.SyncNPCNetworkerGlobals, to: remoteClient, ignore: ignoreClient);
        }
        public static void SendNecromancyProjectile(int remoteClient, int ignoreClient, int projectile)
        {
            Send((p) =>
            {
                p.Write(Main.projectile[projectile].owner);
                p.Write(Main.projectile[projectile].identity);
                var globals = GetNetworkerGlobals(Main.projectile[projectile]);
                for (int i = 0; i < globals.Length; i++)
                {
                    globals[i].Send(p);
                }
            },
            PacketType.SyncNecromanyProjectile, to: remoteClient, ignore: ignoreClient);
        }

        public static INetworker[] GetNetworkerGlobals(NPC npc)
        {
            return new INetworker[] { npc.GetGlobalNPC<NecromancyNPC>() };
        }
        public static INetworker[] GetNetworkerGlobals(Projectile projectile)
        {
            return new INetworker[] { projectile.GetGlobalProjectile<NecromancyProj>() };
        }

        public static PacketType ReadPacketType(BinaryReader reader)
        {
            return (PacketType)reader.ReadByte();
        }

        public static void Send(Action<ModPacket> action, PacketType type, int capacity = 256, int to = -1, int ignore = -1)
        {
            var packet = Aequus.Instance.GetPacket(capacity);
            packet.Write((byte)type);
            action(packet);
            packet.Send(to, ignore);
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
    }
}