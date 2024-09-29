using Aequus.Common.Net;
using System.IO;

namespace Aequus.Content.Entities.PotionAffixes.Empowered;

public class EmpoweredPotionPlayer : ModPlayer {
    public int empowered;

    public override void Load() {
        On_Player.QuickBuff_ShouldBotherUsingThisBuff += On_Player_QuickBuff_ShouldBotherUsingThisBuff;
    }

    static bool On_Player_QuickBuff_ShouldBotherUsingThisBuff(On_Player.orig_QuickBuff_ShouldBotherUsingThisBuff orig, Player self, int attemptedType) {
        if (!orig(self, attemptedType)) {
            return false;
        }

        return !self.TryGetModPlayer(out EmpoweredPotionPlayer potionPlayer) || potionPlayer.empowered == 0 || potionPlayer.empowered == attemptedType;
    }

    public override void ResetEffects() {
        if (empowered != 0 && !Player.HasBuff(empowered)) {
            empowered = 0;
        }
    }
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