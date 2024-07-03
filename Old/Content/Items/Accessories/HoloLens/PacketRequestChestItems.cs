using System.IO;
using Terraria.ModLoader.IO;
using tModLoaderExtended.Networking;

namespace Aequu2.Old.Content.Items.Accessories.HoloLens;

public class PacketRequestChestItems : PacketHandler {
    public void Send(int player, int chestId) {
        if (Main.netMode == NetmodeID.MultiplayerClient) {
            ModPacket packet = GetPacket();
            packet.Write(player);
            packet.Write(chestId);
            packet.Send();
        }
    }

    public void SendServer(int chestId, int toClient) {
        ModPacket packet = GetPacket();
        packet.Write(chestId);
        Chest chest = Main.chest[chestId];
        for (int i = 0; i < Chest.maxItems; i++) {
            ItemIO.Send(chest.item[i], packet, writeStack: true);
        }
        packet.Send(toClient: toClient);
    }

    public override void Receive(BinaryReader reader, int sender) {
        if (Main.netMode == NetmodeID.Server) {
            int player = reader.ReadInt32();
            int chestId = reader.ReadInt32();
            if (Main.chest[chestId] != null) {
                SendServer(chestId, player);
            }
        }
        else {
            int chestID = reader.ReadInt32();
            if (Main.chest[chestID].item == null) {
                Main.chest[chestID].item = new Item[Chest.maxItems];
            }
            for (int i = 0; i < Chest.maxItems; i++) {
                Main.chest[chestID].item[i] = ItemIO.Receive(reader, readStack: true);
            }
        }
    }
}
