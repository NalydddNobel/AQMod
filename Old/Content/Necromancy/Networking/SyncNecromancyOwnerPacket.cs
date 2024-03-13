using Aequus.Core.Networking;
using System.IO;

namespace Aequus.Old.Content.Necromancy.Networking;

public class SyncNecromancyOwnerPacket : PacketHandler {
    public void Send(int npc, int player) {
        ModPacket p = GetPacket();
        p.Write((byte)npc);
        p.Write((byte)player);
        p.Send();
    }

    public override void Receive(BinaryReader reader, int sender) {
        byte n = reader.ReadByte();
        byte p = reader.ReadByte();
        if (!Main.npc.IndexInRange(n) || !Main.npc[n].active || !Main.npc[n].TryGetGlobalNPC(out NecromancyNPC necromancyNPC)
            || !Main.player.IndexInRange(p) || !Main.player[p].active) {
            return; // :(
        }

        necromancyNPC.zombieOwner = p;

        if (Main.netMode == NetmodeID.Server) {
            Send(n, p);
        }
    }
}
