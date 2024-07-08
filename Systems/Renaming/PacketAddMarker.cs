using System.IO;
using tModLoaderExtended.Networking;

namespace AequusRemake.Systems.Renaming;

public sealed class PacketAddMarker : PacketHandler {
    public void Send(int markerId, RenamedNPCMarker marker) {
        var packet = GetPacket();
        packet.Write(markerId);
        marker.NetSend(packet);
        packet.Send();
    }

    public override void Receive(BinaryReader reader, int sender) {
        int markerId = reader.ReadInt32();
        if (RenamingSystem.RenamedNPCs.TryGetValue(markerId, out var value)) {
            value.NetReceive(reader);
        }
        else {
            var marker = new RenamedNPCMarker();
            marker.NetReceive(reader);
            RenamingSystem.RenamedNPCs.Add(markerId, marker);
        }
    }
}