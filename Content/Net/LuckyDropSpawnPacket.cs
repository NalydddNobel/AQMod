using Aequus.Common.Net;
using Aequus.Items;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ID;

namespace Aequus.Content.Net {
    public class LuckyDropSpawnPacket : PacketHandler {
        public override PacketType LegacyPacketType => PacketType.LuckyDropSpawn;

        public void Send(Vector2 location, int width, int height, int amount, int ignore = -1) {
            var p = GetLegacyPacket();
            p.Write(location.X);
            p.Write(location.Y);
            p.Write(AsClampedByte(width));
            p.Write(AsClampedByte(height));
            p.Write(AsClampedByte(amount));
            p.Send(ignoreClient: ignore);
        }

        public override void Receive(BinaryReader reader, int sender) {
            float x = reader.ReadSingle();
            float y = reader.ReadSingle();
            byte width = reader.ReadByte();
            byte height = reader.ReadByte();
            byte amount = reader.ReadByte();

            if (Main.netMode == NetmodeID.Server) {
                Send(new(x, y), width, height, amount, ignore: sender);
                return;
            }

            AequusItem.LuckyDropSpawnEffect(new(x, y), width, height, amount);
        }
    }
}