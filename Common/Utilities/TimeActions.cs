using AQMod.Effects;
using System.Reflection;
using Terraria;
using Terraria.ID;

namespace AQMod.Common.Utilities
{
    public static class TimeActions
    {
        public static class Hooks
        {
            internal static MethodInfo Main_UpdateTime_SpawnTownNPCs;

            internal static void Main_UpdateTime(On.Terraria.Main.orig_UpdateTime orig)
            {
                AQSystem.UpdatingTime = true;
                Main.dayRate += AQSystem.DayrateIncrease;
                if (Main.dayTime)
                {
                    if (Main.time + Main.dayRate > Main.dayLength)
                    {
                        AQSystem.CosmicanonActive = AQPlayer.IgnoreMoons();
                        AprilFoolsJoke.UpdateActive();
                        if (Main.netMode != NetmodeID.Server)
                        {
                            SkyGlimmerEvent.InitNight();
                        }
                    }
                    orig();
                    AQSystem.CosmicanonActive = false;
                }
                else
                {
                    if (Main.time + Main.dayRate > Main.nightLength)
                    {
                        AQSystem.CosmicanonActive = AQPlayer.IgnoreMoons();
                    }
                    orig();
                    if (WorldDefeats.TownNPCMoveAtNight && !Main.dayTime)
                    {
                        Main_UpdateTime_SpawnTownNPCs.Invoke(null, null);
                    }
                    AQSystem.CosmicanonActive = false;
                }
                AQSystem.DayrateIncrease = 0;
                MessageBroadcast.PreventChat = false;
                MessageBroadcast.PreventChatOnce = false;
                AQSystem.UpdatingTime = false;
            }
        }

        public const double MaxTime = Main.dayLength + Main.nightLength;
        public const double HourMultiplier = 60d * 60d;
        public const double FourHoursThirtyMinutes = HourMultiplier * 4.5d;

        public static double GetInGameTime()
        {
            return Main.dayTime ? Main.time : Main.dayLength + Main.time;
        }

        public static double GetInGameTimePercentage()
        {
            return GetInGameTime() / MaxTime;
        }

        public static double GetInGameTimePercentageUsing12AMAs0Percent()
        {
            double time = GetInGameTime() + FourHoursThirtyMinutes;
            if (time > MaxTime)
            {
                time -= MaxTime; // wrap around
            }
            return time / MaxTime;
        }
    }
}