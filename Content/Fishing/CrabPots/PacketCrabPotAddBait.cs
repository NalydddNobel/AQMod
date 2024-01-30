using Aequus.Core.Graphics.Animations;
using Aequus.Core.Networking;
using System.IO;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;

namespace Aequus.Content.Fishing.CrabPots;

public class PacketCrabPotAddBait : PacketHandler {
    public void Send(System.Int32 x, System.Int32 y, System.Int32 player, Item bait, System.Int32 ignoreClient = -1) {
        var packet = GetPacket();
        packet.Write((System.UInt16)x);
        packet.Write((System.UInt16)y);
        packet.Write((System.Byte)player);
        ItemIO.Send(bait, packet);
        packet.Send(ignoreClient: ignoreClient);
    }

    public override void Receive(BinaryReader reader, System.Int32 sender) {
        var x = reader.ReadUInt16();
        var y = reader.ReadUInt16();
        var plr = reader.ReadByte();
        var bait = ItemIO.Receive(reader);

        if (!TileEntity.ByPosition.TryGetValue(new(x, y), out var te) || te is not TECrabPot crabPot) {
            return;
        }

        if (Main.netMode == NetmodeID.Server) {
            Send(x, y, plr, bait, sender);
        }
        else {
            AnimationSystem.GetValueOrAddDefault<AnimationOpenCrabPot>(x, y);
        }
        if (crabPot.item.IsAir) {
            crabPot.ClearItem();
            crabPot.item = bait.Clone();
        }
    }
}