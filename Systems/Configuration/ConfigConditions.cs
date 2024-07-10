using System;
using System.Reflection;
using Terraria.Localization;
using Terraria.ModLoader.Config;

namespace AequusRemake.Content.Configuration;

internal class ConfigConditions {
    /// <returns><inheritdoc cref="GetCondition(ModConfig, string, LocalizedText, Func{object, bool})"/> Defaults to checking if the value equals true.</returns>
    public static Condition IsTrue(ModConfig config, string settingName) {
        return GetCondition(config, settingName, Language.GetText("Mods.AequusRemake.Condition.ConfigIsTrue"), (value) => value.Equals(true));
    }

    /// <returns>A condition which depends on a certain config setting.</returns>
    internal static Condition GetCondition(ModConfig config, string settingName, LocalizedText description, Func<object, bool> CheckSetting) {
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
}
