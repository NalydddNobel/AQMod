using Aequus.Common.Net;
using System.IO;

namespace Aequus.Content.Entities.PotionAffixes.Mistral;

public class EmpoweredPotionPlayer : ModPlayer {
    public int empowered;
}

public class EmpoweredPotionPlayerPacket : PacketHandler {
    public override PacketType LegacyPacketType => PacketType.EmpoweredPotionPlayer;

    public void Send(Player player, EmpoweredPotionPlayer potionPlayer, int toWho = -1, int fromWho = -1) {
        var packet = GetPacket();

        packet.Write((byte)player.whoAmI);

        lock (potionPlayer) {
            packet.Write(potionPlayer.empowered);
        }

        SendPacket(packet, toWho, fromWho);
    }

    public override void Receive(BinaryReader reader, int sender) {
        byte plr = reader.ReadByte();

        int empoweredPotionId = reader.ReadInt32();

        if (!Main.player[plr].active || !Main.player[plr].TryGetModPlayer(out EmpoweredPotionPlayer potionPlayer)) {
            return;
        }

        potionPlayer.empowered = empoweredPotionId;
    }
}