﻿using Aequus.Common.Net;
using Aequus.Core;
using Aequus.Core.Graphics.Animations;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Aequus.Content.Tiles.CrabPots;
public class PacketCrabPotUse : PacketHandler {
    public void Send(int x, int y, int player, int waterStyleId) {
        var packet = GetPacket();
        packet.Write((ushort)x);
        packet.Write((ushort)y);
        packet.Write((byte)player);
        LiquidsSystem.SendWaterStyle(packet, waterStyleId);
        packet.Send();
    }

    public override void Receive(BinaryReader reader, int sender) {
        var x = reader.ReadUInt16();
        var y = reader.ReadUInt16();
        var plr = reader.ReadByte();
        int waterStyle = LiquidsSystem.ReceiveWaterStyle(reader);

        if (!TileEntity.ByPosition.TryGetValue(new(x, y), out var te) || te is not TECrabPot crabPot) {
            return;
        }

        if (Main.myPlayer == plr) {
            CrabPot.GrabItem(x, y, plr, crabPot);
        }
        if (Main.netMode == NetmodeID.Server) {
            Send(x, y, plr, waterStyle);
        }
        else {
            AnimationSystem.GetValueOrAddDefault<AnimationCrabPot>(x, y);
        }
        crabPot.biomeData = new(waterStyle);
        crabPot.item.TurnToAir();
    }
}