using Aequus.Common.Net;
using Aequus.Core;
using System.IO;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Aequus.Content.Tiles.CrabPots;

public class PacketCrabPotPlacement : PacketHandler {
    public void Send(int x, int y, int waterStyleId) {
        var packet = GetPacket();
        packet.Write((ushort)x);
        packet.Write((ushort)y);
        LiquidsSystem.SendWaterStyle(packet, waterStyleId);
        packet.Send();
    }

    public override void Receive(BinaryReader reader, int sender) {
        var x = reader.ReadUInt16();
        var y = reader.ReadUInt16();
        LiquidsSystem.WaterStyle = LiquidsSystem.ReceiveWaterStyle(reader);
        TileEntity.PlaceEntityNet(x, y, ModContent.GetInstance<TECrabPot>().Type);
    }
}