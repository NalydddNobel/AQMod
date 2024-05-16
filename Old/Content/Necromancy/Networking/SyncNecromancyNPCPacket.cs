using System.IO;
using tModLoaderExtended.Networking;

namespace Aequus.Old.Content.Necromancy.Networking;

public class SyncNecromancyNPCPacket : PacketHandler {
    public void Send(NPC npc) {
        if (!npc.TryGetGlobalNPC(out NecromancyNPC necromancyNPC)) {
            return;
        }

        ModPacket p = GetPacket();
        p.Write((byte)npc.whoAmI);
        necromancyNPC.Send(p);
        p.Send();
    }

    public override void Receive(BinaryReader reader, int sender) {
        byte n = reader.ReadByte();
        if (!Main.npc.IndexInRange(n) || !Main.npc[n].active || !Main.npc[n].TryGetGlobalNPC(out NecromancyNPC necromancyNPC)) {
            return; // :(
        }

        necromancyNPC.Receive(reader);

        if (Main.netMode == NetmodeID.Server) {
            Send(Main.npc[n]);
        }
    }
}
