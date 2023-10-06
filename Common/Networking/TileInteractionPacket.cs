using Aequus.Common.Net;
using Aequus.Common.Tiles.Components;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Common.Networking;

public class TileInteractionPacket : PacketHandler {
    public void Send(int i, int j) {
        var packet = GetPacket();
        packet.Write(i);
        packet.Write(j);
        packet.Write(Main.tile[i, j].TileType);
        if (TileLoader.GetTile(Main.tile[i, j].TileType) is INetTileInteraction netTileInteractions) {
            netTileInteractions.Send(i, j, packet);
        }
        packet.Send();
    }

    public override void Receive(BinaryReader reader, int sender) {
        int i = reader.ReadInt32();
        int j = reader.ReadInt32();
        ushort type = reader.ReadUInt16();
        if (TileLoader.GetTile(type) is INetTileInteraction netTileInteractions) {
            netTileInteractions.Receive(i, j, reader, sender);
        }
    }
}