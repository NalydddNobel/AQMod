using Aequus.Core.Networking;
using System.IO;

namespace Aequus.Common.Renaming;

public sealed class PacketAddMarker : PacketHandler {
    public void Send(System.Int32 markerId, RenamedNPCMarker marker) {
        var packet = GetPacket();
        packet.Write(markerId);
        marker.NetSend(packet);
        packet.Send();
    }

    public override void Receive(BinaryReader reader, System.Int32 sender) {
        System.Int32 markerId = reader.ReadInt32();
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