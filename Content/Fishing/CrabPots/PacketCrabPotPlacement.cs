﻿using Aequus.Common;
using Aequus.Common.Net;
using System.IO;
using Terraria.DataStructures;

namespace Aequus.Content.Fishing.CrabPots;

public class PacketCrabPotPlacement : PacketHandler {
    public override PacketType LegacyPacketType => PacketType.CrabPotPlace;

    public void Send(int x, int y, int waterStyleId) {
        var packet = GetPacket();
        packet.Write((ushort)x);
        packet.Write((ushort)y);
        LiquidsSystem.SendWaterStyle(packet, waterStyleId);
        SendPacket(packet);
    }

    public override void Receive(BinaryReader reader, int sender) {
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