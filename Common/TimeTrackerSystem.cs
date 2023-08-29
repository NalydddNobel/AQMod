using System;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Common;

public class TimeTrackerSystem : ModSystem {
    public static int daysPassed;

    public static int WeekDay => daysPassed % 7;

    public static DayOfWeek DayOfWeek => (DayOfWeek)WeekDay;

    public override void Load() {
        On_Main.UpdateTime_StartDay += On_Main_UpdateTime_StartDay;
        On_Main.UpdateTime_StartNight += On_Main_UpdateTime_StartNight;
    }

    private static void On_Main_UpdateTime_StartNight(On_Main.orig_UpdateTime_StartNight orig, ref bool stopEvents) {
        orig(ref stopEvents);
    }

    private static void On_Main_UpdateTime_StartDay(On_Main.orig_UpdateTime_StartDay orig, ref bool stopEvents) {
        daysPassed++;
        orig(ref stopEvents);
    }
}