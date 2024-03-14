using Aequus.Core.Networking;
using System;
using System.IO;

namespace Aequus.Content.Weather.Rain;

public class RainPlayer : ModPlayer {
    public byte rainTotems;

    public override void PreUpdate() {
        if (Main.myPlayer == Player.whoAmI) {
            rainTotems = (byte)Math.Min(RainTotem.RainTotemCount, RainTotem.RainTotemMax);
        }
    }

    public override void SyncPlayer(int toWho, int fromWho, bool newPlayer) {
        Aequus.GetPacket<RainPlayerSyncPacket>().Send(this, toWho, fromWho);
    }

    public override void CopyClientState(ModPlayer targetCopy) {
        RainPlayer clone = (RainPlayer)targetCopy;
        clone.rainTotems = rainTotems;
    }

    public override void SendClientChanges(ModPlayer clientPlayer) {
        RainPlayer clone = (RainPlayer)clientPlayer;
        if (clone.rainTotems != rainTotems) {
            SyncPlayer(toWho: -1, fromWho: Main.myPlayer, newPlayer: false);
        }
    }
}

public class RainPlayerSyncPacket : PacketHandler {
    public void Send(RainPlayer rainPlayer, int toWho = -1, int fromWho = -1) {
        ModPacket packet = GetPacket();
        packet.Write((byte)rainPlayer.Player.whoAmI);
        packet.Write(rainPlayer.rainTotems);
        packet.Send(toWho, fromWho);
    }

    public override void Receive(BinaryReader reader, int sender) {
        byte player = reader.ReadByte();
        Main.player[player].GetModPlayer<RainPlayer>().rainTotems = reader.ReadByte();
    }
}