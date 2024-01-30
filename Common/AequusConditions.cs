using Aequus.Common.Systems;
using System;
using Terraria.Localization;

namespace Aequus.Common;

public class AequusConditions {
    public static Condition BetweenDays(DayOfWeek firstDay, DayOfWeek lastDay) {
        var actualFirstDay = firstDay;
        var actualLastDay = lastDay;
        if (lastDay < firstDay) {
            actualFirstDay = lastDay;
            actualLastDay = firstDay;
        }
        return NewCondition(Language.GetText("Mods.Aequus.Condition.BetweenDays").WithFormatArgs(ExtendLanguage.DayOfWeek(firstDay), ExtendLanguage.DayOfWeek(lastDay)), () => TimeTrackerSystem.DayOfTheWeek >= firstDay && TimeTrackerSystem.DayOfTheWeek <= lastDay);
    }

    public static Condition DayOfTheWeek(DayOfWeek dayOfWeek) {
        return NewCondition(Language.GetText("Mods.Aequus.Condition.DayOfTheWeek").WithFormatArgs(ExtendLanguage.DayOfWeek(dayOfWeek)), () => TimeTrackerSystem.DayOfTheWeek == dayOfWeek);
    }

    private static Condition NewCondition(string name, Func<bool> predicate) {
        return NewCondition(Language.GetOrRegister("Mods.Aequus.Condition." + name), predicate);
    }

    private static Condition NewCondition(LocalizedText name, Func<bool> predicate) {
        return new(name, predicate);
    }
}