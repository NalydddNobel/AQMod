using Aequus.Common.Systems;
using System;
using System.Reflection;
using Terraria.Localization;
using Terraria.ModLoader.Config;

namespace Aequus.Common;

public partial class Commons {
    public class Conditions {
        #region Downed X
        /// <summary>Whether ??? was defeated.</summary>
        public static readonly Condition DownedAquaticBoss = new Condition("Mods.Aequus.Condition.DownedAquaticBoss", () => WorldState.DownedAquaticBoss);
        /// <summary>Whether ??? was not defeated.</summary>
        public static readonly Condition NotDownedAquaticBoss = new Condition("Mods.Aequus.Condition.NotDownedAquaticBoss", () => !WorldState.DownedAquaticBoss);

        /// <summary>Whether Ultra Starite was defeated.</summary>
        public static readonly Condition DownedCosmicBoss = new Condition("Mods.Aequus.Condition.DownedCosmicBoss", () => WorldState.DownedCosmicBoss);
        /// <summary>Whether Ultra Starite not was defeated.</summary>
        public static readonly Condition NotDownedCosmicBoss = new Condition("Mods.Aequus.Condition.NotDownedCosmicBoss", () => !WorldState.DownedCosmicBoss);

        /// <summary>Whether Omega Starite was defeated.</summary>
        public static readonly Condition DownedTrueCosmicBoss = new Condition("Mods.Aequus.Condition.DownedTrueCosmicBoss", () => WorldState.DownedTrueCosmicBoss);
        /// <summary>Whether Omega Starite not was defeated.</summary>
        public static readonly Condition NotDownedTrueCosmicBoss = new Condition("Mods.Aequus.Condition.NotDownedTrueCosmicBoss", () => !WorldState.DownedTrueCosmicBoss);

        /// <summary>Whether the Demon Siege was completed atleast once.</summary>
        public static readonly Condition DownedDemonBoss = new Condition("Mods.Aequus.Condition.DownedDemonBoss", () => WorldState.DownedDemonBoss);
        /// <summary>Whether the Demon Siege not was completed atleast once.</summary>
        public static readonly Condition NotDownedDemonBoss = new Condition("Mods.Aequus.Condition.NotDownedDemonBoss", () => !WorldState.DownedDemonBoss);

        /// <summary>Whether Upriser (Unimplemented) was defeated.</summary>
        public static readonly Condition DownedTrueDemonBoss = new Condition("Mods.Aequus.Condition.DownedTrueDemonBoss", () => WorldState.DownedTrueDemonBoss);
        /// <summary>Whether Upriser (Unimplemented) not was defeated.</summary>
        public static readonly Condition NotDownedTrueDemonBoss = new Condition("Mods.Aequus.Condition.NotDownedTrueDemonBoss", () => !WorldState.DownedTrueDemonBoss);

        /// <summary>Whether Red Sprite was defeated.</summary>
        public static readonly Condition DownedAtmosphereBossFlame = new Condition("Mods.Aequus.Condition.DownedAtmosphereBossFlame", () => WorldState.DownedAtmoBossFlame);
        /// <summary>Whether Red Sprite was not defeated.</summary>
        public static readonly Condition NotDownedAtmosphereBossFlame = new Condition("Mods.Aequus.Condition.NotDownedAtmosphereBossFlame", () => !WorldState.DownedAtmoBossFlame);

        /// <summary>Whether Space Squid was defeated.</summary>
        public static readonly Condition DownedAtmosphereBossFrost = new Condition("Mods.Aequus.Condition.DownedAtmosphereBossFrost", () => WorldState.DownedAtmoBossFrost);
        /// <summary>Whether Space Squid was not defeated.</summary>
        public static readonly Condition NotDownedAtmosphereBossFrost = new Condition("Mods.Aequus.Condition.NotDownedAtmosphereBossFrost", () => !WorldState.DownedAtmoBossFrost);

        /// <summary>Whether Dust Devil was defeated.</summary>
        public static readonly Condition DownedTrueAtmosphereBoss = new Condition("Mods.Aequus.Condition.DownedTrueAtmosphereBoss", () => WorldState.DownedTrueAtmoBoss);
        /// <summary>Whether Dust Devil was not defeated.</summary>
        public static readonly Condition NotDownedTrueAtmosphereBoss = new Condition("Mods.Aequus.Condition.NotDownedTrueAtmosphereBoss", () => !WorldState.DownedTrueAtmoBoss);

        /// <summary>Whether ??? was defeated.</summary>
        public static readonly Condition DownedOrganicBossMight = new Condition("Mods.Aequus.Condition.DownedOrganicBossMight", () => WorldState.DownedOrganicBossMight);
        /// <summary>Whether ??? was not defeated.</summary>
        public static readonly Condition NotDownedOrganicBossMight = new Condition("Mods.Aequus.Condition.NotDownedOrganicBossMight", () => !WorldState.DownedOrganicBossMight);

        /// <summary>Whether ??? was defeated.</summary>
        public static readonly Condition DownedOrganicBossSight = new Condition("Mods.Aequus.Condition.DownedOrganicBossSight", () => WorldState.DownedOrganicBossSight);
        /// <summary>Whether ??? was not defeated.</summary>
        public static readonly Condition NotDownedOrganicBossSight = new Condition("Mods.Aequus.Condition.NotDownedOrganicBossSight", () => !WorldState.DownedOrganicBossSight);

        /// <summary>Whether ??? was defeated.</summary>
        public static readonly Condition DownedOrganicBossFright = new Condition("Mods.Aequus.Condition.DownedOrganicBossFright", () => WorldState.DownedOrganicBossFright);
        /// <summary>Whether ??? was not defeated.</summary>
        public static readonly Condition NotDownedOrganicBossFright = new Condition("Mods.Aequus.Condition.NotDownedOrganicBossFright", () => !WorldState.DownedOrganicBossFright);

        /// <summary>Whether ??? was defeated.</summary>
        public static readonly Condition DownedTrueFinalBoss = new Condition("Mods.Aequus.Condition.DownedTrueFinalBoss", () => WorldState.DownedTrueFinalBoss);
        /// <summary>Whether ??? was not defeated.</summary>
        public static readonly Condition NotDownedTrueFinalBoss = new Condition("Mods.Aequus.Condition.NotDownedTrueFinalBoss", () => !WorldState.DownedTrueFinalBoss);
        #endregion

        #region Days
        /// <param name="firstDay">A <see cref="DayOfWeek"/> to compare against <see cref="TimeSystem.DayOfTheWeek"/>.</param>
        /// <param name="lastDay">A <see cref="DayOfWeek"/> to compare against <see cref="TimeSystem.DayOfTheWeek"/>.</param>
        /// <returns>A condition for a specific time range between two days of the week.</returns>
        public static Condition BetweenDays(DayOfWeek firstDay, DayOfWeek lastDay) {
            LocalizedText text = Language.GetText("Mods.Aequus.Condition.BetweenDays")
                .WithFormatArgs(ExtendLanguage.DayOfWeek(firstDay), ExtendLanguage.DayOfWeek(lastDay));

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

        /// <param name="dayOfWeek">The <see cref="DayOfWeek"/> to compare against <see cref="TimeSystem.DayOfTheWeek"/>.</param>
        /// <returns>A condition for a specific day of the week.</returns>
        public static Condition DayOfTheWeek(DayOfWeek dayOfWeek) {
            return new Condition(Language.GetText("Mods.Aequus.Condition.DayOfTheWeek").WithFormatArgs(ExtendLanguage.DayOfWeek(dayOfWeek)),
                () => TimeSystem.DayOfTheWeek == dayOfWeek);
        }
        #endregion

        #region Config
        /// <returns><inheritdoc cref="Config(ModConfig, string, LocalizedText, Func{object, bool})"/> Defaults to checking if the value equals true.</returns>
        public static Condition ConfigIsTrue(ModConfig config, string settingName) {
            return Config(config, settingName, Language.GetText("Mods.Aequus.Condition.ConfigIsTrue"), (value) => value.Equals(true));
        }
        /// <returns>A condition which depends on a certain config setting.</returns>
        public static Condition Config(ModConfig config, string settingName, LocalizedText description, Func<object, bool> CheckSetting) {
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
}
