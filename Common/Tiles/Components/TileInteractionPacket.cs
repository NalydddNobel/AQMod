using Aequus.Core.Networking;
using System.IO;

namespace Aequus.Common.Tiles.Components;

public class TileInteractionPacket : PacketHandler {
    public void Send(System.Int32 i, System.Int32 j, System.Int32 toClient = -1, System.Int32 ignoreClient = -1) {
        var packet = GetPacket();
        packet.Write(i);
        packet.Write(j);
        packet.Write(Main.tile[i, j].TileType);
        if (TileLoader.GetTile(Main.tile[i, j].TileType) is INetTileInteraction netTileInteractions) {
            netTileInteractions.Send(i, j, packet);
        }
        packet.Send(toClient, ignoreClient);
    }

    public override void Receive(BinaryReader reader, System.Int32 sender) {
        System.Int32 i = reader.ReadInt32();
        System.Int32 j = reader.ReadInt32();
        System.UInt16 type = reader.ReadUInt16();
        if (TileLoader.GetTile(type) is INetTileInteraction netTileInteractions) {
            netTileInteractions.Receive(i, j, reader, sender);
        }
    }
}