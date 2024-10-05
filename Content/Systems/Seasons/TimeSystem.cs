using Aequus.Common.Utilities;
using Aequus.Common.Utilities.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria.Localization;
using Terraria.ModLoader.IO;

namespace Aequus.Content.Systems.Seasons;

public class TimeSystem : ModSystem {
    public const string Tag_DaysPassed = "DaysPassed";
    public const int WeekLength = 7;

    public static readonly int SeasonLength = 30;

    public static int YearLength => SeasonLength * (int)Seasons.Count;

    public static readonly int HalloweenChance = 7;
    public static readonly int XmasChance = 7;

    public int DaysPassed { get; private set; }

    public int WeekDay { get; private set; }

    public Seasons Season { get; private set; }

    public DayOfWeek DayOfTheWeek => (DayOfWeek)WeekDay;

    public int Year => DaysPassed / YearLength;

    public Dictionary<DayOfWeek, LocalizedText> WeekText = [];
    public Dictionary<Seasons, LocalizedText> SeasonText = [];

    public void SetDay(int day) {
        DaysPassed = day;

        if (Main.remixWorld) {
            SetRemixDay();
        }
        else {
            WeekDay = DaysPassed % WeekLength;
            Season = (Seasons)(DaysPassed % YearLength / SeasonLength);
        }

        void SetRemixDay() {
            if (Main.netMode == NetmodeID.Server) {
                DateTime now = DateTime.Now;
                WeekDay = (int)now.DayOfWeek;

                // Only compensates for the northern hemisphere, but this is a dumb easter egg effect anyways.
                int seasonMonth = now.Month;

                // Between December and February
                if (seasonMonth >= 12 || seasonMonth <= 2) {
                    Season = Seasons.Winter;
                }
                // Between March and May
                if (seasonMonth >= 3 || seasonMonth <= 5) {
                    Season = Seasons.Spring;
                }
                // Between June and August
                if (seasonMonth >= 6 || seasonMonth <= 8) {
                    Season = Seasons.Summer;
                }
                // Between September and December
                else {
                    Season = Seasons.Autumn;
                }
            }
        }
    }

    /// <returns>Localized name of a <see cref="DayOfWeek"/> value.</returns>
    public static LocalizedText GetWeekText(DayOfWeek dayOfWeek) {
        return ALanguage.GetText("Misc.DayOfTheWeek." + dayOfWeek.ToString());
    }

    public override void Load() {
        On_Main.UpdateTime_StartDay += On_Main_UpdateTime_StartDay;

        foreach (DayOfWeek day in Enum.GetValues<DayOfWeek>()) {
            WeekText[day] = ALanguage.GetText($"Misc.DayOfTheWeek.{day}");
        }

        foreach (Seasons season in Enum.GetValues<Seasons>().Where(s => s < Seasons.Count)) {
            SeasonText[season] = ALanguage.GetText($"Misc.Season.{season}");
        }
    }

    private static void On_Main_UpdateTime_StartDay(On_Main.orig_UpdateTime_StartDay orig, ref bool stopEvents) {
        TimeSystem time = Instance<TimeSystem>();
        time.SetDay(time.DaysPassed + 1);

        orig(ref stopEvents);

        if (stopEvents || Main.eclipse) {
            return;
        }

        switch (time.Season) {
            case Seasons.Autumn: {
                    if (!Main.halloween && !Main.forceHalloweenForToday && Main.rand.NextBool(HalloweenChance)) {
                        WorldGen.BroadcastText(NetworkText.FromKey("Misc.StartedVictoryHalloween"), CommonColor.TextEvent);
                    }
                }
                break;

            case Seasons.Winter: {
                    if (!Main.xMas && !Main.forceXMasForToday && Main.rand.NextBool(XmasChance)) {
                        WorldGen.BroadcastText(NetworkText.FromKey("Misc.StartedVictoryXmas"), CommonColor.TextEvent);
                    }
                }
                break;
        }
    }

    public override void ClearWorld() {
        DaysPassed = 0;
    }

    public override void SaveWorldData(TagCompound tag) {
        tag[Tag_DaysPassed] = DaysPassed;
    }

    public override void LoadWorldData(TagCompound tag) {
        if (tag.TryGet(Tag_DaysPassed, out int value)) {
            SetDay(value);
        }
    }

    public override void NetSend(BinaryWriter writer) {
        writer.Write(DaysPassed);
        if (Main.remixWorld) {
            writer.Write((byte)WeekDay);
            writer.Write((byte)Season);
        }
    }

    public override void NetReceive(BinaryReader reader) {
        DaysPassed = reader.ReadInt32();
        if (Main.remixWorld) {
            WeekDay = reader.ReadByte();
            Season = (Seasons)reader.ReadByte();
        }
    }
}