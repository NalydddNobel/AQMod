using Aequus.Common.Net;
using System.IO;

namespace Aequus.Systems.Renaming;

public sealed class PacketAddMarker : PacketHandler {
    public override PacketType LegacyPacketType => PacketType.AddNewNameTagMarker;

    public void Send(int markerId, RenamedNPCMarker marker) {
        var packet = GetLegacyPacket();
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