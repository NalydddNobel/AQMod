using Aequus.Common.Net;
using System.IO;
using Terraria;

namespace Aequus.Items.Equipment.Accessories.Money.FoolsGoldRing;

public class FoolsGoldRingEffectPacket : PacketHandler {
    public override PacketType LegacyPacketType => PacketType.FoolsGoldRingEffect;

    public void Send(NPC npc) {
        var p = GetLegacyPacket();
        p.Write((int)npc.position.X);
        p.Write((int)npc.position.Y);
        p.Write(npc.width);
        p.Write(npc.height);
        p.Send();
    }

    public override void Receive(BinaryReader reader) {
        int x = reader.ReadInt32();
        int y = reader.ReadInt32();
        int w = reader.ReadInt32();
        int h = reader.ReadInt32();

        FoolsGoldRing.OnKillEffects(x, y, w, h);
    }
}