using Aequus.Core.Networking;
using System.IO;

namespace Aequus.Content.Tiles.CraftingStations.TrashCompactor;

public class PacketTrashCompactorItemAnimation : PacketHandler {
    public void Send(int x, int y, int totalAmount, int itemType, int ignoreClient = -1) {
        var packet = GetPacket();
        packet.Write((ushort)x);
        packet.Write((ushort)y);
        packet.Write(totalAmount);
        packet.Write(itemType);
        packet.Send(ignoreClient: ignoreClient);
    }

    public override void Receive(BinaryReader reader, int sender) {
        var x = reader.ReadUInt16();
        var y = reader.ReadUInt16();
        int totalAmount = reader.ReadInt32();
        int itemType = reader.ReadInt32();

        TrashCompactor.UseItemAnimation(x, y, totalAmount, itemType);
        if (Main.netMode == NetmodeID.Server) {
            Send(x, y, totalAmount, itemType, ignoreClient: sender);
        }
    }
}