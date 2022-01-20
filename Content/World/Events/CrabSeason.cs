using Microsoft.Xna.Framework;
using System.IO;
using Terraria;

namespace AQMod.Content.World.Events
{
    public sealed class CrabSeason : WorldEvent
    {
        public static Color TextColor => new Color(18, 226, 213, 255);

        public static int crabSeasonTimer;
        public static int crabSeasonTimerRate = 1;
        public const int CrabSeasonTimerMax = 162000;
        public const int CrabSeasonTimerMin = 72000;

        public static bool Active => crabSeasonTimer < 0;
        public static short CrabsonCachedID { get; set; } = -1;

        public static bool InActiveZone(Player player)
        {
            return Active && CrabsonCachedID == -1 && player.ZoneBeach;
        }

        public static void UpdateWorld()
        {
            if (crabSeasonTimer > 0)
            {
                if (crabSeasonTimer - crabSeasonTimerRate <= 0)
                {
                    Activate();
                }
            }
            crabSeasonTimer -= crabSeasonTimerRate;
            if (crabSeasonTimer <= -CrabSeasonTimerMin)
            {
                Deactivate();

            }
            crabSeasonTimerRate = 1;
        }

        public static void Activate()
        {
            if (crabSeasonTimer > 0)
                crabSeasonTimer = 0;
        }

        public static void Deactivate()
        {
            if (crabSeasonTimer < 0)
                crabSeasonTimer = Main.rand.Next(CrabSeasonTimerMin, CrabSeasonTimerMax);
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(crabSeasonTimer);
        }

        public override void NetReceive(BinaryReader reader)
        {
            crabSeasonTimer = reader.ReadInt32();
        }
    }
}