using Aequus.Core.Graphics.Animations;
using tModLoaderExtended.Networking;
using System.IO;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;

namespace Aequus.Content.Fishing.CrabPots;

public class PacketCrabPotAddBait : PacketHandler {
    public void Send(int x, int y, int player, Item bait, int ignoreClient = -1) {
        var packet = GetPacket();
        packet.Write((ushort)x);
        packet.Write((ushort)y);
        packet.Write((byte)player);
        ItemIO.Send(bait, packet);
        packet.Send(ignoreClient: ignoreClient);
    }

    public override void Receive(BinaryReader reader, int sender) {
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