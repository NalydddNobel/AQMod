using System.IO;
using Terraria;
using Terraria.ID;

namespace AQMod.Common.NetCode
{
    public static class NetworkingMethods
    {
        public static void Write(this BinaryWriter writer, NetType message)
        {
            writer.Write((byte)message);
        }

        public static NetType GetMessage(BinaryReader packet)
        {
            return (NetType)packet.ReadByte();
        }

        public static void GlimmerEventNetSummonOmegaStarite()
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                AQMod.spawnStarite = true;
            }
            else
            {
                var p = AQMod.Instance.GetPacket();
                p.Write(NetType.SummonOmegaStarite);
            }
        }

        public static void GlimmerEventNetUpdate()
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                return;
            var p = AQMod.Instance.GetPacket();
            p.Write(NetType.UpdateGlimmerEvent);

            p.Write(AQMod.CosmicEvent.tileX);
            p.Write(AQMod.CosmicEvent.tileY);
            p.Write(AQMod.CosmicEvent.spawnChance);
            p.Write(AQMod.CosmicEvent.StariteDisco);
            p.Write(AQMod.CosmicEvent.deactivationTimer);
        }
    }
}