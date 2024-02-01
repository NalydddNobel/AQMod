using Aequus.Common.Systems;
using System;
using Terraria.Localization;

namespace Aequus;

public partial class Aequus {
    /// <param name="firstDay">A <see cref="DayOfWeek"/> to compare against <see cref="TimeTrackerSystem.DayOfTheWeek"/>.</param>
    /// <param name="lastDay">A <see cref="DayOfWeek"/> to compare against <see cref="TimeTrackerSystem.DayOfTheWeek"/>.</param>
    /// <returns>A condition for a specific time range between two days of the week.</returns>
    public static Condition ConditionBetweenDays(DayOfWeek firstDay, DayOfWeek lastDay) {
        LocalizedText text = Language.GetText("Mods.Aequus.Condition.BetweenDays")
            .WithFormatArgs(ExtendLanguage.DayOfWeek(firstDay), ExtendLanguage.DayOfWeek(lastDay));

        if (firstDay > lastDay) {
            // For example, if something were to be sold between Friday (5) and Monday (1)
            // We should instead check if the day is <= Monday and >= Friday
            // Making the valid days be 5, 6, 0, and 1.
            return NewCondition(text, () => TimeTrackerSystem.DayOfTheWeek <= lastDay && TimeTrackerSystem.DayOfTheWeek >= firstDay);
        }

        // Otherwise, no special logic is needed, we can just check
        // if the current day is between the first and last days.
        return NewCondition(text, () => TimeTrackerSystem.DayOfTheWeek >= firstDay && TimeTrackerSystem.DayOfTheWeek <= lastDay);
    }

    /// <param name="dayOfWeek">The <see cref="DayOfWeek"/> to compare against <see cref="TimeTrackerSystem.DayOfTheWeek"/>.</param>
    /// <returns>A condition for a specific day of the week.</returns>
    public static Condition ConditionDayOfTheWeek(DayOfWeek dayOfWeek) {
        return NewCondition(Language.GetText("Mods.Aequus.Condition.DayOfTheWeek").WithFormatArgs(ExtendLanguage.DayOfWeek(dayOfWeek)),
            () => TimeTrackerSystem.DayOfTheWeek == dayOfWeek);
    }

    private static Condition NewCondition(LocalizedText name, Func<bool> predicate) {
        return new(name, predicate);
    }
}