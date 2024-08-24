using Aequus.Common.Net;
using System.IO;

namespace Aequus.Systems.Renaming;

public sealed class PacketRemoveMarker : PacketHandler {
    public override PacketType LegacyPacketType => PacketType.RemoveNewNameTagMarker;

    public void Send(int markerId) {
        var packet = GetLegacyPacket();
        packet.Write(markerId);
        packet.Send();
    }

    public override void Receive(BinaryReader reader, int sender) {
        int markerId = reader.ReadInt32();
        RenamingSystem.Remove(markerId, Main.netMode != NetmodeID.Server);
    }
}