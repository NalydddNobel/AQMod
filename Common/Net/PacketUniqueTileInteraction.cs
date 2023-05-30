using Aequus.Tiles;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Common.Net {
    public class PacketUniqueTileInteraction : PacketHandler
    {
        public override PacketType LegacyPacketType => PacketType.UniqueTileInteraction;

        public void Send(Player player, int i, int j)
        {
            var p = GetPacket();
            p.Write((byte)player.whoAmI);
            p.Write((ushort)i);
            p.Write((ushort)j);
            p.Send(ignoreClient: player.whoAmI);
        }

        public override void Receive(BinaryReader reader)
        {
            byte player = reader.ReadByte();
            ushort i = reader.ReadUInt16();
            ushort j = reader.ReadUInt16();
            if (!WorldGen.InWorld(i, j) || Main.tile[i, j].TileType < TileID.Count || !Main.player[player].active)
            {
                return;
            }

            if (TileLoader.GetTile(Main.tile[i, j].TileType) is TileHooks.IUniqueTileInteractions tileInteractions)
            {
                tileInteractions.Interact(Main.player[player], i, j);
            }
        }
    }
}