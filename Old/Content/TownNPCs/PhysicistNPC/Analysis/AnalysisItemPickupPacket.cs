using System.IO;
using tModLoaderExtended.Networking;

namespace Aequus.Old.Content.TownNPCs.PhysicistNPC.Analysis;

public class AnalysisItemPickupPacket : PacketHandler {
    public void Send(Player player, Item item) {
        SendInner(item.OriginalRarity, ContentSamples.ItemsByType[item.type].value);
    }

    private void SendInner(int rarity, int value) {
        ModPacket packet = GetPacket();
        packet.Write(rarity);
        packet.Write(value);
        packet.Send();
    }

    public override void Receive(BinaryReader reader, int sender) {
        int rarity = reader.ReadInt32();
        int value = reader.ReadInt32();
        if (Main.netMode == NetmodeID.MultiplayerClient) {
            SendInner(rarity, value);
        }
        else {
            AnalysisSystem.HandleItemPickup(rarity, value);
        }
    }
}
