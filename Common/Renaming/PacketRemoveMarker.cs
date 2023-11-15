using Aequus.Common.Net;
using System.IO;
using Terraria;
using Terraria.ID;

namespace Aequus.Common.Renaming;

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