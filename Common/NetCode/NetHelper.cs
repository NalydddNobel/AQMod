using AQMod.Content;
using AQMod.Content.World.Events.GlimmerEvent;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

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
                p.Send();
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
            p.Write(GlimmerEvent.StariteDisco);
            p.Write(GlimmerEvent.deactivationTimer);

            p.Send();
        }

        public static void PreventedBloodMoon()
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                return;
            var p = AQMod.Instance.GetPacket();
            p.Write(AQPacketID.PreventedBloodMoon);
            p.Write(CosmicanonCounts.BloodMoonsPrevented);
            p.Send();
        }

        public static void PreventedGlimmer()
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                return;
            var p = AQMod.Instance.GetPacket();
            p.Write(AQPacketID.PreventedGlimmer);
            p.Write(CosmicanonCounts.GlimmersPrevented);
            p.Send();
        }

        public static void PreventedEclipse()
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                return;
            var p = AQMod.Instance.GetPacket();
            p.Write(AQPacketID.PreventedEclipse);
            p.Write(CosmicanonCounts.EclipsesPrevented);
            p.Send();
        }

        public static void BeginDemonSiege(int x, int y, int plr, Item item)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                return;
            var p = AQMod.Instance.GetPacket();
            p.Write(AQPacketID.BeginDemonSiege);
            p.Write(x);
            p.Write(y);
            p.Write(plr);
            p.Write(item.netID);
            p.Write(item.stack);
            p.Write(item.prefix);
            if (item.type > Main.maxItemTypes)
            {
                item.modItem.NetSend(p);
            }
            p.Send();
        }
    }
}