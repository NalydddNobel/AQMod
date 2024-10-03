using Aequus.Common.Net;
using System.IO;
using Terraria.ModLoader.IO;

namespace Aequus.Content.Systems.Keys.Keychains;

public class KeychainPlayerPacket : PacketHandler {
    public override PacketType LegacyPacketType => PacketType.Keychain;

    public void SendSingleItem(int p, int slot) {
        var packet = GetPacket();

        Player player = Main.player[p];
        KeychainPlayer keychain = player.GetModPlayer<KeychainPlayer>();

        packet.Write((byte)p);
        lock (keychain) {
            packet.Write((sbyte)(-slot - 1));
            ItemIO.Send(keychain.keys[slot], packet, writeStack: true);
        }

        SendPacket(packet);
    }

    public void Send(int p) {
        var packet = GetPacket();

        Player player = Main.player[p];
        KeychainPlayer keychain = player.GetModPlayer<KeychainPlayer>();

        packet.Write((byte)p);
        lock (keychain) {
            packet.Write((sbyte)keychain.keys.Count);
            for (int i = 0; i < keychain.keys.Count; i++) {
                ItemIO.Send(keychain.keys[i], packet, writeStack: true);
            }
        }

        SendPacket(packet);
    }

    public override void Receive(BinaryReader reader, int sender) {
        int p = reader.ReadByte();
        Player player = Main.player[p];
        KeychainPlayer keychain = player.GetModPlayer<KeychainPlayer>();
        int value = reader.ReadSByte();

        // From: SendSingleItem
        if (value < 0) {
            value++;
            value = -value;

            lock (keychain) {
                if (keychain.keys.Count <= value) {
                    keychain.keys.Add(ItemIO.Receive(reader, readStack: true));
                }
                else {
                    ItemIO.Receive(keychain.keys[value], reader, readStack: true);
                }
            }
        }
        else {
            lock (keychain) {
                keychain.keys.Clear();

                for (int i = 0; i < value; i++) {
                    keychain.keys.Add(ItemIO.Receive(reader, readStack: true));
                }
            }
        }

        keychain.RefreshKeys();
    }
}
