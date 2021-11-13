using AQMod.Items.Tools;
using AQMod.Localization;
using Microsoft.Xna.Framework;
using Terraria;

namespace AQMod.Content.WorldEvents.AquaticEvent
{
    public static class CrabSeason
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
                    if (Main.LocalPlayer.HeldItem.modItem is CrabClock)
                        AQMod.BroadcastMessage(AQText.ModText("Common.CrabSeasonWarning").Value, TextColor);
                }
            }
            crabSeasonTimer -= crabSeasonTimerRate;
            if (crabSeasonTimer <= -CrabSeasonTimerMin)
            {
                Deactivate();
                if (Main.LocalPlayer.HeldItem.modItem is CrabClock)
                    AQMod.BroadcastMessage(AQText.ModText("Common.CrabSeasonEnding").Value, TextColor);
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
    }
}