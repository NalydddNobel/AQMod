using System;

namespace AequusRemake.Core.Structures.Conditions;

internal static class ACondition {
    public static readonly Condition InGlimmer = New("InGlimmer", () => false);

    public static Condition New(string Key, Func<bool> Condition) {
        return new Condition($"Mods.AequusRemake.Condition.{Key}", Condition);
    }
}
