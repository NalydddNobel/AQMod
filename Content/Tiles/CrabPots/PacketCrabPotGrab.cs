using Aequus.Common.Net;
using Aequus.Core.Graphics.Animations;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Tiles.CrabPots;
public class PacketCrabPotGrab : PacketHandler {
    public void Send(int x, int y, int player, int waterStyleId) {
        var packet = GetPacket();
        packet.Write((ushort)x);
        packet.Write((ushort)y);
        packet.Write((byte)player);
        packet.Write(waterStyleId);
        if (waterStyleId >= Main.maxLiquidTypes) {
            packet.Write(LoaderManager.Get<WaterFallStylesLoader>().Get(waterStyleId).FullName);
        }
        packet.Send();
    }

    public override void Receive(BinaryReader reader, int sender) {
        var x = reader.ReadUInt16();
        var y = reader.ReadUInt16();
        var plr = reader.ReadByte();
        int waterStyle = reader.ReadInt32();
        if (waterStyle >= Main.maxLiquidTypes) {
            waterStyle = CrabPotBiomeData.GetWaterStyle(reader.ReadString());
        }

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