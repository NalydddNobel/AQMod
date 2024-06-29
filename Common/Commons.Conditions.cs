using Aequus.Common.Systems;
using System;
using System.Reflection;
using Terraria.Localization;
using Terraria.ModLoader.Config;

namespace Aequus.Common;

public partial class Commons {
    public class Conditions {
        #region Days
        /// <param name="firstDay">A <see cref="DayOfWeek"/> to compare against <see cref="TimeSystem.DayOfTheWeek"/>.</param>
        /// <param name="lastDay">A <see cref="DayOfWeek"/> to compare against <see cref="TimeSystem.DayOfTheWeek"/>.</param>
        /// <returns>A condition for a specific time range between two days of the week.</returns>
        public static Condition BetweenDays(DayOfWeek firstDay, DayOfWeek lastDay) {
            LocalizedText text = Language.GetText("Mods.Aequus.Condition.BetweenDays")
                .WithFormatArgs(XLanguage.DayOfWeek(firstDay), XLanguage.DayOfWeek(lastDay));

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
            return new Condition(Language.GetText("Mods.Aequus.Condition.DayOfTheWeek").WithFormatArgs(XLanguage.DayOfWeek(dayOfWeek)),
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
