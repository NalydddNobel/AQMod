using Aequus.Common.Net;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Aequus.Content.Tiles.CrabPots;

public class PacketCrabPotPlacement : PacketHandler {
    public void Send(int x, int y, int waterStyleId) {
        var packet = GetPacket();
        packet.Write((ushort)x);
        packet.Write((ushort)y);
        packet.Write(waterStyleId);
        if (waterStyleId >= Main.maxLiquidTypes) {
            packet.Write(LoaderManager.Get<WaterFallStylesLoader>().Get(waterStyleId).FullName);
        }
        packet.Send();
    }

    public override void Receive(BinaryReader reader, int sender) {
        var x = reader.ReadUInt16();
        var y = reader.ReadUInt16();
        TECrabPot.WaterStyle = reader.ReadInt32();
        if (TECrabPot.WaterStyle >= Main.maxLiquidTypes) {
            TECrabPot.WaterStyle = CrabPotBiomeData.GetWaterStyle(reader.ReadString());
        }
        TileEntity.PlaceEntityNet(x, y, ModContent.GetInstance<TECrabPot>().Type);
    }
}