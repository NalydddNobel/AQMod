using System.IO;
using Terraria.ModLoader.IO;
using tModLoaderExtended.Networking;

namespace Aequus.Content.Backpacks;

public class BackpackPlayerSyncPacket : PacketHandler {
    /// <summary>Invokes a single item being sent from a backpack.</summary>
    public void SendSingleItem(Player player, BackpackData backpack, int slot, int toWho = -1, int fromWho = -1) {
        ModPacket packet = GetPacket();
        packet.Write((byte)player.whoAmI);
        lock (backpack) {
            packet.Write((byte)backpack.Type);
            packet.Write((sbyte)(-slot - 1)); // Slot hack to signify a single item is needing to be synced.
            WriteItem(packet, backpack, slot);
        }
        packet.Send(toWho, fromWho);
    }

    /// <summary>Invokes a full sync of a backpack.</summary>
    public void Send(Player player, BackpackData backpack, int toWho = -1, int fromWho = -1) {
        ModPacket packet = GetPacket();
        packet.Write((byte)player.whoAmI);
        lock (backpack) {
            packet.Write((byte)backpack.Type);
            packet.Write((sbyte)backpack.Inventory.Length);
            for (int i = 0; i < backpack.Inventory.Length; i++) {
                WriteItem(packet, backpack, i);
            }
        }
        packet.Send(toWho, fromWho);
    }

    public override void Receive(BinaryReader reader, int sender) {
        int playerIndex = reader.ReadByte();
        int backpackType = reader.ReadByte();
        int value = reader.ReadSByte();

        Player player = Main.player[playerIndex];
        BackpackData backpack = player.GetModPlayer<BackpackPlayer>().backpacks[backpackType];

        // From: SendSingleItem
        if (value < 0) {
            // value = slot
            value++; // Offset index by 1
            value = -value; // Make the value positive
            ReadItem(reader, backpack, value);

            if (Main.netMode == NetmodeID.Server) {
                SendSingleItem(player, backpack, value, fromWho: sender);
            }
            return;
        }

        // value = count
        backpack.EnsureCapacity(value);

        for (int i = 0; i < value; i++) {
            ReadItem(reader, backpack, i);
        }

        if (Main.netMode == NetmodeID.Server) {
            Send(player, backpack, fromWho: sender);
        }
    }

    private void WriteItem(BinaryWriter writer, BackpackData backpack, int slot) {
        ItemIO.Send(backpack.Inventory[slot], writer, writeStack: true, writeFavorite: true);
    }

    private void ReadItem(BinaryReader reader, BackpackData backpack, int slot) {
        Item[] inv = backpack.Inventory;
        if (inv.Length <= slot) {
            // Read dummy data
            Item _ = ItemIO.Receive(reader, readStack: true, readFavorite: true);
            return;
        }

        if (inv[slot] == null) {
            inv[slot] = ItemIO.Receive(reader, readStack: true, readFavorite: true);
        }
        else {
            ItemIO.Receive(inv[slot], reader, readStack: true, readFavorite: true);
        }
    }
}