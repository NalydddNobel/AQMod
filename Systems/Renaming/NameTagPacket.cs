using Aequus.Common.Net;
using System.IO;

namespace Aequus.Systems.Renaming;

public class NameTagPacket : PacketHandler {
    public override PacketType LegacyPacketType => PacketType.NewNameTag;

    public void Send(int i, string nameTag) {
        var p = GetLegacyPacket();
        p.Write(i);
        p.Write(nameTag);
        p.Send();
    }

    public override void Receive(BinaryReader reader, int sender) {
        int npcIndex = reader.ReadInt32();
        string nameTag = reader.ReadString();

        NameTag.NametagEffects(npcIndex, nameTag);

        if (Main.netMode == NetmodeID.Server) {
            Send(npcIndex, nameTag);
        }
    }
}