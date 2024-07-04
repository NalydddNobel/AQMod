using System;
using System.IO;
using tModLoaderExtended.Networking;

namespace AequusRemake.Content.Tiles.RainTotem;

public class RainTotemPlayer : ModPlayer {
    public byte rainTotems;

    public override void PreUpdate() {
        if (Main.myPlayer == Player.whoAmI) {
            rainTotems = (byte)Math.Min(RainTotem.RainTotemCount, RainTotem.RainTotemMax);
        }
    }

    public override void SyncPlayer(int toWho, int fromWho, bool newPlayer) {
        GetPacket<RainPlayerSyncPacket>().Send(this, toWho, fromWho);
    }

    public override void CopyClientState(ModPlayer targetCopy) {
        RainTotemPlayer clone = (RainTotemPlayer)targetCopy;
        clone.rainTotems = rainTotems;
    }

    public override void SendClientChanges(ModPlayer clientPlayer) {
        RainTotemPlayer clone = (RainTotemPlayer)clientPlayer;
        if (clone.rainTotems != rainTotems) {
            SyncPlayer(toWho: -1, fromWho: Main.myPlayer, newPlayer: false);
        }
    }
}

public class RainPlayerSyncPacket : PacketHandler {
    public void Send(RainTotemPlayer rainPlayer, int toWho = -1, int fromWho = -1) {
        ModPacket packet = GetPacket();
        packet.Write((byte)rainPlayer.Player.whoAmI);
        packet.Write(rainPlayer.rainTotems);
        packet.Send(toWho, fromWho);
    }

    public override void Receive(BinaryReader reader, int sender) {
        byte player = reader.ReadByte();
        byte rainTotems = reader.ReadByte();
        if (Main.player[player].TryGetModPlayer(out RainTotemPlayer rainPlayer)) {
            rainPlayer.rainTotems = rainTotems;
        }
    }
}