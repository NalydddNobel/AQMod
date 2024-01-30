using Aequus.Core.Networking;
using System.IO;

namespace Aequus.Content.Tools.AnglerLamp;

public class AnglerLampEffectPacket : PacketHandler {
    public void Send(System.Int32 npc) {
        var packet = GetPacket();
        packet.Write(npc);
        packet.Send();
    }

    public override void Receive(BinaryReader reader, System.Int32 sender) {
        System.Int32 npc = reader.ReadInt32();
        if (!Main.npc[npc].active) {
            return;
        }

        if (Main.netMode == NetmodeID.Server) {
            Send(npc);
        }
        else {
            AnglerLamp.LanternHitEffect(npc, quiet: true);
        }
    }
}