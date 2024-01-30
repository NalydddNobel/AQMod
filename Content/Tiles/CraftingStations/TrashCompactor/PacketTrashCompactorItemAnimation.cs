using Aequus.Core.Networking;
using System.IO;

namespace Aequus.Content.Tiles.CraftingStations.TrashCompactor;

public class PacketTrashCompactorItemAnimation : PacketHandler {
    public void Send(System.Int32 x, System.Int32 y, System.Int32 totalAmount, System.Int32 itemType, System.Int32 ignoreClient = -1) {
        var packet = GetPacket();
        packet.Write((System.UInt16)x);
        packet.Write((System.UInt16)y);
        packet.Write(totalAmount);
        packet.Write(itemType);
        packet.Send(ignoreClient: ignoreClient);
    }

    public override void Receive(BinaryReader reader, System.Int32 sender) {
        var x = reader.ReadUInt16();
        var y = reader.ReadUInt16();
        System.Int32 totalAmount = reader.ReadInt32();
        System.Int32 itemType = reader.ReadInt32();

        TrashCompactor.UseItemAnimation(x, y, totalAmount, itemType);
        if (Main.netMode == NetmodeID.Server) {
            Send(x, y, totalAmount, itemType, ignoreClient: sender);
        }
    }
}