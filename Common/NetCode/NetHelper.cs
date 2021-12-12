using AQMod.Content.WorldEvents.GlimmerEvent;
using System.IO;
using Terraria;
using Terraria.ID;

namespace AQMod.Common.NetCode
{
    public static class NetHelper
    {
        public static void GlimmerEventNetSummonOmegaStarite()
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                AQMod.spawnStarite = true;
            }
            else
            {
                var p = AQMod.Instance.GetPacket();
                p.Write(AQPacketID.SummonOmegaStarite);
            }
        }

        public static void GlimmerEventNetUpdate()
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                return;
            var p = AQMod.Instance.GetPacket();
            p.Write(AQPacketID.UpdateGlimmerEvent);

            p.Write(GlimmerEvent.tileX);
            p.Write(GlimmerEvent.tileY);
            p.Write(GlimmerEvent.spawnChance);
            p.Write(GlimmerEvent.StariteDisco);
            p.Write(GlimmerEvent.deactivationTimer);
        }
    }
}