using System;
using Terraria.ModLoader.IO;

namespace Aequus.Common.Systems;

public class TimeTrackerSystem : ModSystem {
    public static int daysPassed;

    public static int WeekDay { get; private set; }

    public static DayOfWeek DayOfTheWeek => (DayOfWeek)WeekDay;

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

    public override void ClearWorld() {
        daysPassed = 0;
    }

    public override void SaveWorldData(TagCompound tag) {
        tag["DaysPassed"] = daysPassed;
    }

    public override void LoadWorldData(TagCompound tag) {
        if (tag.TryGet("DaysPassed", out int value)) {
            daysPassed = value;
        }
    }

    public override void PostUpdateTime() {
        if (Main.zenithWorld) {
            WeekDay = (int)DateTime.Now.DayOfWeek;
        }
        else {
            WeekDay = daysPassed % 7;
        }
    }
}