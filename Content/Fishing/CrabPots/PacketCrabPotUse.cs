using Aequus.Core;
using Aequus.Core.Graphics.Animations;
using Aequus.Core.Networking;
using System.IO;
using Terraria.DataStructures;

namespace Aequus.Content.Fishing.CrabPots;
public class PacketCrabPotUse : PacketHandler {
    public void Send(System.Int32 x, System.Int32 y, System.Int32 player, System.Int32 waterStyleId) {
        var packet = GetPacket();
        packet.Write((System.UInt16)x);
        packet.Write((System.UInt16)y);
        packet.Write((System.Byte)player);
        LiquidsSystem.SendWaterStyle(packet, waterStyleId);
        packet.Send();
    }

    public override void Receive(BinaryReader reader, System.Int32 sender) {
        var x = reader.ReadUInt16();
        var y = reader.ReadUInt16();
        var plr = reader.ReadByte();
        System.Int32 waterStyle = LiquidsSystem.ReceiveWaterStyle(reader);

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
            AnimationSystem.GetValueOrAddDefault<AnimationOpenCrabPot>(x, y);
        }
        crabPot.biomeData = new(waterStyle);
        crabPot.ClearItem();
    }
}