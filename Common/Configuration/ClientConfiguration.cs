using System;
using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace Aequus.Common.Configuration
{
    public sealed class ClientConfiguration : BaseConfiguration
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        public static ClientConfiguration Instance;

        [Header("$Mods.AQMod.ClientConfig.Header.Visuals")]

        [BackgroundColor(140, 29, 47, 180)]
        [Label("$Mods.AQMod.ClientConfig.ScreenshakesLabel")]
        [DefaultValue(true)]
        public bool screenshakes;

        [BackgroundColor(140, 29, 47, 180)]
        [Label("$Mods.AQMod.ClientConfig.EffectIntensityLabel")]
        [DefaultValue(1f)]
        [Range(0.1f, 1f)]
        public float effectIntensity;

        [BackgroundColor(140, 29, 47, 180)]
        [Label("$Mods.AQMod.ClientConfig.FlashIntensityLabel")]
        [DefaultValue(1f)]
        [Range(0.1f, 1f)]
        public float flashIntensity;

        [BackgroundColor(140, 29, 47, 180)]
        [Label("$Mods.AQMod.ClientConfig.EffectQualityLabel")]
        [DefaultValue(1f)]
        [Range(0.1f, 2f)]
        public float effectQuality;
    }
}