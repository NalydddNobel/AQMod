using Aequus.Common.Net;
using System.IO;

namespace Aequus.Content.Necromancy.Networking;

public class SyncNecromancyOwnerPacket : PacketHandler {
    public override PacketType LegacyPacketType => PacketType.SyncNecromancyOwnerPacket;

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
