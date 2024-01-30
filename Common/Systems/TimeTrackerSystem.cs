using System;
using Terraria.ModLoader.IO;

namespace Aequus.Common.Systems;

public class TimeTrackerSystem : ModSystem {
    public static Int32 daysPassed;

    public static Int32 WeekDay { get; private set; }

    public static DayOfWeek DayOfTheWeek => (DayOfWeek)WeekDay;

    public override void Load() {
        On_Main.UpdateTime_StartDay += On_Main_UpdateTime_StartDay;
        On_Main.UpdateTime_StartNight += On_Main_UpdateTime_StartNight;
    }

    private static void On_Main_UpdateTime_StartNight(On_Main.orig_UpdateTime_StartNight orig, ref Boolean stopEvents) {
        orig(ref stopEvents);
    }

    private static void On_Main_UpdateTime_StartDay(On_Main.orig_UpdateTime_StartDay orig, ref Boolean stopEvents) {
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
        if (tag.TryGet("DaysPassed", out Int32 value)) {
            daysPassed = value;
        }
    }

    public override void PostUpdateTime() {
        if (Main.zenithWorld) {
            WeekDay = (Int32)DateTime.Now.DayOfWeek;
        }
        else {
            WeekDay = daysPassed % 7;
        }
    }
}