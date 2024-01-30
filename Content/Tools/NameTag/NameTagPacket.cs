using Aequus.Core.Networking;
using System.IO;

namespace Aequus.Content.Tools.NameTag;

public class NameTagPacket : PacketHandler {
    public void Send(System.Int32 i, System.String nameTag) {
        var p = GetPacket();
        p.Write(i);
        p.Write(nameTag);
        p.Send();
    }

    public override void Receive(BinaryReader reader, System.Int32 sender) {
        System.Int32 npcIndex = reader.ReadInt32();
        System.String nameTag = reader.ReadString();

        NameTag.NametagEffects(npcIndex, nameTag);

        if (Main.netMode == NetmodeID.Server) {
            Send(npcIndex, nameTag);
        }
    }
}