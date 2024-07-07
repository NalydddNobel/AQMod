using System;
using System.IO;
using Terraria.Localization;
using Terraria.ModLoader.IO;

namespace AequusRemake.Core.Systems;

public class TimeSystem : ModSystem {
    public static int DaysPassed { get; private set; }

    public static int WeekDay { get; private set; }

    public static DayOfWeek DayOfTheWeek => (DayOfWeek)WeekDay;

    /// <param name="firstDay">A <see cref="DayOfWeek"/> to compare against <see cref="DayOfTheWeek"/>.</param>
    /// <param name="lastDay">A <see cref="DayOfWeek"/> to compare against <see cref="DayOfTheWeek"/>.</param>
    /// <returns>A condition for a specific time range between two days of the week.</returns>
    public static Condition ConditionBetweenDaysOfWeek(DayOfWeek firstDay, DayOfWeek lastDay) {
        LocalizedText text = XLanguage.GetText("Condition.BetweenDays")
            .WithFormatArgs(GetWeekText(firstDay), GetWeekText(lastDay));

        if (firstDay > lastDay) {
            // For example, if something were to be sold between Friday (5) and Monday (1)
            // We should instead check if the day is <= Monday and >= Friday
            // Making the valid days be 5, 6, 0, and 1.
            return new Condition(text, () => TimeSystem.DayOfTheWeek <= lastDay && TimeSystem.DayOfTheWeek >= firstDay);
        }

        // Otherwise, no special logic is needed, we can just check
        // if the current day is between the first and last days.
        return new Condition(text, () => TimeSystem.DayOfTheWeek >= firstDay && TimeSystem.DayOfTheWeek <= lastDay);
    }

    /// <param name="dayOfWeek">The <see cref="GetWeekText"/> to compare against <see cref="TimeSystem.DayOfTheWeek"/>.</param>
    /// <returns>A condition for a specific day of the week.</returns>
    public static Condition ConditionByDayOfWeek(DayOfWeek dayOfWeek) {
        return new Condition(XLanguage.GetText("Condition.DayOfTheWeek").WithFormatArgs(GetWeekText(dayOfWeek)), () => DayOfTheWeek == dayOfWeek);
    }

    /// <returns>Localized name of a <see cref="System.DayOfWeek"/> value.</returns>
    public static LocalizedText GetWeekText(DayOfWeek dayOfWeek) {
        return XLanguage.GetText("Misc.DayOfTheWeek." + dayOfWeek.ToString());
    }

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