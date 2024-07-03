using System;
using System.IO;
using Terraria.ModLoader.IO;

namespace Aequu2.Core.Systems;

public class TimeSystem : ModSystem {
    public static int DaysPassed { get; private set; }

    public static int WeekDay { get; private set; }

    public static DayOfWeek DayOfTheWeek => (DayOfWeek)WeekDay;

    public override void ClearWorld() {
        DaysPassed = 0;
    }

    public override void SaveWorldData(TagCompound tag) {
        tag["DaysPassed"] = DaysPassed;
    }

    public override void LoadWorldData(TagCompound tag) {
        if (tag.TryGet("DaysPassed", out int value)) {
            DaysPassed = value;
        }
    }

    public override void PostUpdateTime() {
        if (Main.remixWorld) {
            if (Main.netMode == NetmodeID.Server) {
                WeekDay = (int)DateTime.Now.DayOfWeek;
            }
        }
        else {
            WeekDay = DaysPassed % 7;
        }
    }

    public override void NetSend(BinaryWriter writer) {
        writer.Write(DaysPassed);
        writer.Write(WeekDay);
    }

    public override void NetReceive(BinaryReader reader) {
        DaysPassed = reader.ReadInt32();
        WeekDay = reader.ReadInt32();
    }

    internal static void OnStartDay() {
        DaysPassed++;
    }
}