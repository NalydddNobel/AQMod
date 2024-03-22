using Aequus.Common.Systems;
using Aequus.Core;
using System;
using System.Reflection;
using Terraria.Localization;
using Terraria.ModLoader.Config;

namespace Aequus;

public partial class Aequus {
    #region Downed X
    /// <summary>Whether ??? was defeated.</summary>
    public static readonly Condition ConditionDownedAquaticBoss = new Condition("Mods.Aequus.Condition.DownedAquaticBoss", () => WorldState.DownedAquaticBoss);
    /// <summary>Whether ??? was not defeated.</summary>
    public static readonly Condition ConditionNotDownedAquaticBoss = new Condition("Mods.Aequus.Condition.NotDownedAquaticBoss", () => !WorldState.DownedAquaticBoss);
    
    /// <summary>Whether Ultra Starite was defeated.</summary>
    public static readonly Condition ConditionDownedCosmicBoss = new Condition("Mods.Aequus.Condition.DownedCosmicBoss", () => WorldState.DownedCosmicBoss);
    /// <summary>Whether Ultra Starite not was defeated.</summary>
    public static readonly Condition ConditionNotDownedCosmicBoss = new Condition("Mods.Aequus.Condition.NotDownedCosmicBoss", () => !WorldState.DownedCosmicBoss);
    
    /// <summary>Whether Omega Starite was defeated.</summary>
    public static readonly Condition ConditionDownedTrueCosmicBoss = new Condition("Mods.Aequus.Condition.DownedTrueCosmicBoss", () => WorldState.DownedTrueCosmicBoss);
    /// <summary>Whether Omega Starite not was defeated.</summary>
    public static readonly Condition ConditionNotDownedTrueCosmicBoss = new Condition("Mods.Aequus.Condition.NotDownedTrueCosmicBoss", () => !WorldState.DownedTrueCosmicBoss);
    
    /// <summary>Whether the Demon Siege was completed atleast once.</summary>
    public static readonly Condition ConditionDownedDemonBoss = new Condition("Mods.Aequus.Condition.DownedDemonBoss", () => WorldState.DownedDemonBoss);
    /// <summary>Whether the Demon Siege not was completed atleast once.</summary>
    public static readonly Condition ConditionNotDownedDemonBoss = new Condition("Mods.Aequus.Condition.NotDownedDemonBoss", () => !WorldState.DownedDemonBoss);
    
    /// <summary>Whether Upriser (Unimplemented) was defeated.</summary>
    public static readonly Condition ConditionDownedTrueDemonBoss = new Condition("Mods.Aequus.Condition.DownedTrueDemonBoss", () => WorldState.DownedTrueDemonBoss);
    /// <summary>Whether Upriser (Unimplemented) not was defeated.</summary>
    public static readonly Condition ConditionNotDownedTrueDemonBoss = new Condition("Mods.Aequus.Condition.NotDownedTrueDemonBoss", () => !WorldState.DownedTrueDemonBoss);
    
    /// <summary>Whether Red Sprite was defeated.</summary>
    public static readonly Condition ConditionDownedAtmosphereBossFlame = new Condition("Mods.Aequus.Condition.DownedAtmosphereBossFlame", () => WorldState.DownedAtmoBossFlame);
    /// <summary>Whether Red Sprite was not defeated.</summary>
    public static readonly Condition ConditionNotDownedAtmosphereBossFlame = new Condition("Mods.Aequus.Condition.NotDownedAtmosphereBossFlame", () => !WorldState.DownedAtmoBossFlame);
    
    /// <summary>Whether Space Squid was defeated.</summary>
    public static readonly Condition ConditionDownedAtmosphereBossFrost = new Condition("Mods.Aequus.Condition.DownedAtmosphereBossFrost", () => WorldState.DownedAtmoBossFrost);
    /// <summary>Whether Space Squid was not defeated.</summary>
    public static readonly Condition ConditionNotDownedAtmosphereBossFrost = new Condition("Mods.Aequus.Condition.NotDownedAtmosphereBossFrost", () => !WorldState.DownedAtmoBossFrost);
    
    /// <summary>Whether Dust Devil was defeated.</summary>
    public static readonly Condition ConditionDownedTrueAtmosphereBoss = new Condition("Mods.Aequus.Condition.DownedTrueAtmosphereBoss", () => WorldState.DownedTrueAtmoBoss);
    /// <summary>Whether Dust Devil was not defeated.</summary>
    public static readonly Condition ConditionNotDownedTrueAtmosphereBoss = new Condition("Mods.Aequus.Condition.NotDownedTrueAtmosphereBoss", () => !WorldState.DownedTrueAtmoBoss);
   
    /// <summary>Whether ??? was defeated.</summary>
    public static readonly Condition ConditionDownedOrganicBossMight = new Condition("Mods.Aequus.Condition.DownedOrganicBossMight", () => WorldState.DownedOrganicBossMight);
    /// <summary>Whether ??? was not defeated.</summary>
    public static readonly Condition ConditionNotDownedOrganicBossMight = new Condition("Mods.Aequus.Condition.NotDownedOrganicBossMight", () => !WorldState.DownedOrganicBossMight);
   
    /// <summary>Whether ??? was defeated.</summary>
    public static readonly Condition ConditionDownedOrganicBossSight = new Condition("Mods.Aequus.Condition.DownedOrganicBossSight", () => WorldState.DownedOrganicBossSight);
    /// <summary>Whether ??? was not defeated.</summary>
    public static readonly Condition ConditionNotDownedOrganicBossSight = new Condition("Mods.Aequus.Condition.NotDownedOrganicBossSight", () => !WorldState.DownedOrganicBossSight);
    
    /// <summary>Whether ??? was defeated.</summary>
    public static readonly Condition ConditionDownedOrganicBossFright = new Condition("Mods.Aequus.Condition.DownedOrganicBossFright", () => WorldState.DownedOrganicBossFright);
    /// <summary>Whether ??? was not defeated.</summary>
    public static readonly Condition ConditionNotDownedOrganicBossFright = new Condition("Mods.Aequus.Condition.NotDownedOrganicBossFright", () => !WorldState.DownedOrganicBossFright);
    
    /// <summary>Whether ??? was defeated.</summary>
    public static readonly Condition ConditionDownedTrueFinalBoss = new Condition("Mods.Aequus.Condition.DownedTrueFinalBoss", () => WorldState.DownedTrueFinalBoss);
    /// <summary>Whether ??? was not defeated.</summary>
    public static readonly Condition ConditionNotDownedTrueFinalBoss = new Condition("Mods.Aequus.Condition.NotDownedTrueFinalBoss", () => !WorldState.DownedTrueFinalBoss);
    #endregion

    #region Days
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
    #endregion

    #region Config
    /// <returns><inheritdoc cref="ConditionConfig(ModConfig, string, LocalizedText, Func{object, bool})"/> Defaults to checking if the value equals true.</returns>
    public static Condition ConditionConfigIsTrue(ModConfig config, string settingName) {
        return ConditionConfig(config, settingName, Language.GetText("Mods.Aequus.Condition.ConfigIsTrue"), (value) => value.Equals(true));
    }
    /// <returns>A condition which depends on a certain config setting.</returns>
    public static Condition ConditionConfig(ModConfig config, string settingName, LocalizedText description, Func<object, bool> CheckSetting) {
        Type type = config.GetType();

        Func<bool> predicate = GetPredicate();

        return new Condition(description.WithFormatArgs(
                config.GetLocalization($"{settingName}.Label"),
                config.DisplayName
            ),
            predicate
        );

        Func<bool> GetPredicate() {
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance;

            FieldInfo field = type.GetField(settingName, bindingFlags);
            if (field != null) {
                return () => CheckSetting(field.GetValue(config));
            }

            PropertyInfo property = type.GetProperty(settingName, bindingFlags);
            if (property != null) {
                return () => CheckSetting(property.GetValue(config));
            }

            throw new MissingMemberException(config.Name, settingName);
        }
    }
    #endregion
}