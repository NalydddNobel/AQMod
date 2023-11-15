using Aequus.Common.Net;
using System.IO;
using Terraria;
using Terraria.ID;

namespace Aequus.Content.Items.Tools.AnglerLamp;

public class AnglerLampEffectPacket : PacketHandler {
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
            AnglerLamp.LanternHitEffect(npc, quiet: true);
        }
    }
}