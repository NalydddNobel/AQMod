using Aequus.Common.Systems;
using System;
using Terraria;

namespace Aequus.Common;

public class AequusConditions {
    public static Condition DayOfTheWeek(DayOfWeek dayOfWeek) {
        return NewCondition("DayOfTheWeek." + dayOfWeek.ToString(), () => TimeTrackerSystem.DayOfTheWeek == dayOfWeek);
    }

    private static Condition NewCondition(string name, Func<bool> predicate) {
        return new("Mods.Aequus.Condition." + name, predicate);
    }
}