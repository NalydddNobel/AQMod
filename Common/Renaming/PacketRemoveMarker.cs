using Aequus.Core.Networking;
using System.IO;

namespace Aequus.Common.Renaming;

public sealed class PacketRemoveMarker : PacketHandler {
    public void Send(System.Int32 markerId) {
        var packet = GetPacket();
        packet.Write(markerId);
        packet.Send();
    }

    public override void Receive(BinaryReader reader, System.Int32 sender) {
        System.Int32 markerId = reader.ReadInt32();
        RenamingSystem.Remove(markerId, Main.netMode != NetmodeID.Server);
    }
}