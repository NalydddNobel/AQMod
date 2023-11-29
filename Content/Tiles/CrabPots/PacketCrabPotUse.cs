using Aequus.Core;
using Aequus.Core.Graphics.Animations;
using Aequus.Core.Networking;
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
            BaseCrabPot.GrabItem(x, y, plr, crabPot);
        }
        if (Main.netMode == NetmodeID.Server) {
            Send(x, y, plr, waterStyle);
        }
        else {
            AnimationSystem.GetValueOrAddDefault<AnimationCrabPot>(x, y);
        }
        crabPot.biomeData = new(waterStyle);
        crabPot.ClearItem();
    }
}