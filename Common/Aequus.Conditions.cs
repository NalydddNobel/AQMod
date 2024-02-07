using Aequus.Common.Systems;
using Aequus.Core;
using System;
using Terraria.Localization;

namespace Aequus;

public partial class Aequus {
    /// <summary>Whether ??? was defeated.</summary>
    public static readonly Condition ConditionDownedAquaticBoss = new Condition("Mods.Aequus.Condition.DownedAquaticBoss", () => true);
    /// <summary>Whether Ultra Starite was defeated.</summary>
    public static readonly Condition ConditionDownedCosmicBoss = new Condition("Mods.Aequus.Condition.DownedCosmicBoss", () => true);
    /// <summary>Whether Omega Starite was defeated.</summary>
    public static readonly Condition ConditionDownedTrueCosmicBoss = new Condition("Mods.Aequus.Condition.DownedTrueCosmicBoss", () => true);
    /// <summary>Whether the Demon Siege was completed atleast once.</summary>
    public static readonly Condition ConditionDownedDemonBoss = new Condition("Mods.Aequus.Condition.DownedDemonBoss", () => WorldFlags.DownedDemonSiegeT1);
    /// <summary>Whether Upriser (Unimplemented) was defeated.</summary>
    public static readonly Condition ConditionDownedTrueDemonBoss = new Condition("Mods.Aequus.Condition.DownedTrueDemonBoss", () => true);
    /// <summary>Whether Red Sprite was defeated.</summary>
    public static readonly Condition ConditionDownedAtmosphereBossFlame = new Condition("Mods.Aequus.Condition.DownedAtmosphereBossFlame", () => true);
    /// <summary>Whether Space Squid was defeated.</summary>
    public static readonly Condition ConditionDownedAtmosphereBossFrost = new Condition("Mods.Aequus.Condition.DownedAtmosphereBossFrost", () => true);
    /// <summary>Whether Dust Devil was defeated.</summary>
    public static readonly Condition ConditionDownedTrueAtmosphereBoss = new Condition("Mods.Aequus.Condition.DownedTrueAtmosphereBoss", () => true);
    /// <summary>Whether ??? was defeated.</summary>
    public static readonly Condition ConditionDownedOrganicBossMight = new Condition("Mods.Aequus.Condition.DownedOrganicBossMight", () => true);
    /// <summary>Whether ??? was defeated.</summary>
    public static readonly Condition ConditionDownedOrganicBossSight = new Condition("Mods.Aequus.Condition.DownedOrganicBossSight", () => true);
    /// <summary>Whether ??? was defeated.</summary>
    public static readonly Condition ConditionDownedOrganicBossFright = new Condition("Mods.Aequus.Condition.DownedOrganicBossFright", () => true);
    /// <summary>Whether ??? was defeated.</summary>
    public static readonly Condition ConditionDownedTrueFinalBoss = new Condition("Mods.Aequus.Condition.DownedTrueFinalBoss", () => true);

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
            return new Condition(text, () => TimeTrackerSystem.DayOfTheWeek <= lastDay && TimeTrackerSystem.DayOfTheWeek >= firstDay);
        }

        // Otherwise, no special logic is needed, we can just check
        // if the current day is between the first and last days.
        return new Condition(text, () => TimeTrackerSystem.DayOfTheWeek >= firstDay && TimeTrackerSystem.DayOfTheWeek <= lastDay);
    }

    /// <param name="dayOfWeek">The <see cref="DayOfWeek"/> to compare against <see cref="TimeTrackerSystem.DayOfTheWeek"/>.</param>
    /// <returns>A condition for a specific day of the week.</returns>
    public static Condition ConditionDayOfTheWeek(DayOfWeek dayOfWeek) {
        return new Condition(Language.GetText("Mods.Aequus.Condition.DayOfTheWeek").WithFormatArgs(ExtendLanguage.DayOfWeek(dayOfWeek)),
            () => TimeTrackerSystem.DayOfTheWeek == dayOfWeek);
    }
}