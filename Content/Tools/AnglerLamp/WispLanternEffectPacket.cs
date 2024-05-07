using tModLoaderExtended.Networking;
using System.IO;

namespace Aequus.Content.Tools.AnglerLamp;

public class WispLanternEffectPacket : PacketHandler {
    public void Send(int npc) {
        var packet = GetPacket();
        packet.Write(npc);
        packet.Send();
    }

    public override void Receive(BinaryReader reader, int sender) {
        int npc = reader.ReadInt32();
        if (!Main.npc[npc].active) {
            return;
        }

        if (Main.netMode == NetmodeID.Server) {
            Send(npc);
        }
        else {
            WispLantern.LanternHitEffect(npc, quiet: true);
        }
    }
}