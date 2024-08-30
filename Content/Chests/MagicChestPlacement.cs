using Aequus.Common.Net;
using System.IO;

namespace Aequus.Content.Chests;

public class MagicChestPlacementEffect : PacketHandler {
    public override PacketType LegacyPacketType => PacketType.MagicChestPlacement;

    public void NewEffect(int x, int y) {
        Vector2 effectPosition = new Vector2(x, y).ToWorldCoordinates();
        foreach (Player p in Main.ActivePlayers) {
            if (Helper.InWatcherView(p.Center, effectPosition)) {
                Send(x, y, toClient: p.whoAmI);
            }
        }
    }

    public void Send(int x, int y, int toClient = -1) {
        var p = GetPacket();
        p.Write(x);
        p.Write(y);
        SendPacket(p, toClient);
    }

    public override void Receive(BinaryReader reader, int sender) {
        int x = reader.ReadInt32();
        int y = reader.ReadInt32();

        for (int i = 0; i < 60; i++) {
            Dust.NewDust(new Vector2(x * 16, y * 16), 32, 32, DustID.Smoke, 0f, -1.5f, Scale: Main.rand.NextFloat(1f, 2f));
        }
    }
}
