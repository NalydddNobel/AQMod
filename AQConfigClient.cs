using AQMod.Localization;
using Microsoft.Xna.Framework;
using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace AQMod
{
    [Label("$Mods.AQMod.ClientConfig.Name")]
    [BackgroundColor(10, 10, 40, 220)]
    public class AQConfigClient : ModConfig
    {
        public static AQConfigClient Instance;

        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Header("$Mods.AQMod.ClientConfig.Header.Visuals")]

        [BackgroundColor(140, 29, 47, 180)]
        [Label("$Mods.AQMod.ClientConfig.Screenshakes")]
        [DefaultValue(true)]
        public bool Screenshakes { get; set; }

        [BackgroundColor(140, 29, 47, 180)]
        [Label("$Mods.AQMod.ClientConfig.EffectIntensity")]
        [DefaultValue(1f)]
        [Range(0.1f, 1f)]
        public float EffectIntensity { get; set; }

        [BackgroundColor(140, 29, 47, 180)]
        [Label("$Mods.AQMod.ClientConfig.FlashIntensity")]
        [DefaultValue(1f)]
        [Range(0.1f, 1f)]
        public float FlashIntensity { get; set; }

        [BackgroundColor(140, 29, 47, 180)]
        [Label("$Mods.AQMod.ClientConfig.EffectQuality")]
        [DefaultValue(1f)]
        [Range(0.1f, 2f)]
        public float EffectQuality { get; set; }

        [Header("$Mods.AQMod.ClientConfig.Header.UI")]

        [BackgroundColor(12, 44, 190, 180)]
        [Label("$Mods.AQMod.ClientConfig.DemonSiegeUpgradeTooltip")]
        [Tooltip("$Mods.AQMod.ClientConfig.DemonSiegeUpgradeTooltipTooltip")]
        [DefaultValue(true)]
        public bool DemonSiegeUpgradeTooltip { get; set; }

        [BackgroundColor(12, 44, 190, 180)]
        [Label("$Mods.AQMod.ClientConfig.HookBarbBlacklistTooltip")]
        [Tooltip("$Mods.AQMod.ClientConfig.HookBarbBlacklistTooltipTooltip")]
        [DefaultValue(true)]
        public bool HookBarbBlacklistTooltip { get; set; }

        [Label("$Mods.AQMod.ClientConfig.ShowCompletedAnglerQuestsCount")]
        [BackgroundColor(12, 44, 190, 180)]
        [DefaultValue(true)]
        public bool ShowCompletedQuestsCount { get; set; }

        [BackgroundColor(12, 44, 190, 180)]
        [Label("$Mods.AQMod.UIConfiguration.MapUITogglesPosition")]
        [Tooltip("$Mods.AQMod.UIConfiguration.MapUITogglesPositionTooltip")]
        [Range(60f, 1920f)]
        [DefaultValue(typeof(Vector2), "60, 60")]
        public Vector2 MapUITogglesPosition { get; set; }

        [Header(AQText.ConfigHeaderKey + "Misc")]

        [BackgroundColor(75, 80, 100, 180)]
        [Label("$Mods.AQMod.ClientConfig.XmasBackground")]
        [Tooltip("$Mods.AQMod.ClientConfig.XmasBackgroundTooltip")]
        [DefaultValue(true)]
        [ReloadRequired()]
        public bool XmasBackground { get; set; }

        [BackgroundColor(75, 80, 100, 180)]
        [Label("$Mods.AQMod.ClientConfig.XmasProgressMeterOverride")]
        [Tooltip("$Mods.AQMod.ClientConfig.XmasProgressMeterOverrideTooltip")]
        [DefaultValue(true)]
        [ReloadRequired()]
        public bool XmasProgressMeterOverride { get; set; }
    }
}