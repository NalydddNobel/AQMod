using Aequus.Core;
using Aequus.Core.Networking;
using System.IO;
using Terraria.DataStructures;

namespace Aequus.Content.Fishing.CrabPots;

public class PacketCrabPotPlacement : PacketHandler {
    public void Send(System.Int32 x, System.Int32 y, System.Int32 waterStyleId) {
        var packet = GetPacket();
        packet.Write((System.UInt16)x);
        packet.Write((System.UInt16)y);
        LiquidsSystem.SendWaterStyle(packet, waterStyleId);
        packet.Send();
    }

    public override void Receive(BinaryReader reader, System.Int32 sender) {
        var x = reader.ReadUInt16();
        var y = reader.ReadUInt16();
        if (Main.netMode == NetmodeID.Server) {
            LiquidsSystem.WaterStyle = LiquidsSystem.ReceiveWaterStyle(reader);
            TileEntity.PlaceEntityNet(x, y, ModContent.GetInstance<TECrabPot>().Type);
            Send(x, y, LiquidsSystem.WaterStyle);
        }
        else {
            TECrabPot.PlacementEffects(x, y);
        }
    }
}