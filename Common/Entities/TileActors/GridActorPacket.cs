using Aequus.Common.Net;
using System.IO;

namespace Aequus.Common.Entities.TileActors;

public class GridActorPacket : PacketHandler {
    public override PacketType LegacyPacketType => PacketType.GridActors;

    public void Send(GridActor actor) {
        var packet = GetPacket();

        packet.Write(actor.Location.X);
        packet.Write(actor.Location.Y);
        packet.Write(actor.Type);
        packet.Write(actor.Id);

        actor.SendData(packet);

        SendPacket(packet);
    }

    public override void Receive(BinaryReader reader, int sender) {
        int x = reader.ReadInt32();
        int y = reader.ReadInt32();
        byte type = reader.ReadByte();
        uint id = reader.ReadUInt32();

        Instance<GridActorSystem>().PlaceManual(new Point(x, y), type, id).ReceiveData(reader);
    }
}
