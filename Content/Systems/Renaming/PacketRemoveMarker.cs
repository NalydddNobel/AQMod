using System.IO;
using tModLoaderExtended.Networking;

namespace Aequus.Content.Systems.Renaming;

public sealed class PacketRemoveMarker : PacketHandler {
    public void Send(int markerId) {
        var packet = GetPacket();
        packet.Write(markerId);
        packet.Send();
    }

    public override void Receive(BinaryReader reader, int sender) {
        int markerId = reader.ReadInt32();
        RenamingSystem.Remove(markerId, Main.netMode != NetmodeID.Server);
    }
}