using Aequus.Common.Net;
using System.IO;
using Terraria;
using Terraria.ID;

namespace Aequus.Items.Accessories.CrownOfBlood;

public class WormScarfDodgePacket : PacketHandler {
    public override PacketType LegacyPacketType => PacketType.WormScarfDodge;

    public void Send(Player player) {
        var p = GetPacket();
        p.Write((byte)player.whoAmI);
        p.Send();
    }

    public override void Receive(BinaryReader reader, int sender) {
        byte plr = reader.ReadByte();
        Main.player[plr].Aequus().ProcWormScarfDodge();
        if (Main.netMode == NetmodeID.Server) {
            Send(Main.player[plr]);
        }
    }
}