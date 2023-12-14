﻿using Aequus.Core;
using Aequus.Core.Networking;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Fishing.CrabPots;

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