using Aequus.Common.Structures.Conditions;
using Aequus.Common.Utilities;
using System;
using Terraria.Localization;

namespace Aequus.Content.Systems.Seasons;

public class TimeCondition {
    /// <param name="firstDay">A <see cref="DayOfWeek"/> to compare against <see cref="TimeSystem.DayOfTheWeek"/>.</param>
    /// <param name="lastDay">A <see cref="DayOfWeek"/> to compare against <see cref="TimeSystem.DayOfTheWeek"/>.</param>
    /// <returns>A condition for a specific time range between two days of the week.</returns>
    public static Condition BetweenWeekdays(DayOfWeek firstDay, DayOfWeek lastDay) {
        TimeSystem time = Instance<TimeSystem>();

        LocalizedText firstDayText = time.WeekText.GetOrDefault(firstDay, ALanguage.UnknownText);
        LocalizedText lastDayText = time.WeekText.GetOrDefault(lastDay, ALanguage.UnknownText);

        LocalizedText text = ACondition.GetText("BetweenDays").WithFormatArgs(firstDayText, lastDayText);

        if (firstDay > lastDay) {
            // For example, if something were to be sold between Friday (5) and Monday (1)
            // We should instead check if the day is <= Monday and >= Friday
            // Making the valid days be 5, 6, 0, and 1.
            return new Condition(text, () => Instance<TimeSystem>().DayOfTheWeek <= lastDay || Instance<TimeSystem>().DayOfTheWeek >= firstDay);
        }

        // Otherwise, no special logic is needed, we can just check
        // if the current day is between the first and last days.
        return new Condition(text, () => Instance<TimeSystem>().DayOfTheWeek >= firstDay && Instance<TimeSystem>().DayOfTheWeek <= lastDay);
    }

    /// <param name="weekDay">The <see cref="DayOfWeek"/> to compare against <see cref="TimeSystem.DayOfTheWeek"/>.</param>
    /// <returns>A condition for a specific day of the week.</returns>
    public static Condition Weekday(DayOfWeek weekDay) {
        TimeSystem time = Instance<TimeSystem>();
        LocalizedText text = time.WeekText.GetOrDefault(weekDay, ALanguage.UnknownText);

        return new Condition(ACondition.GetText("DayOfTheWeek").WithFormatArgs(text), () => Instance<TimeSystem>().DayOfTheWeek == weekDay);
    }
}
