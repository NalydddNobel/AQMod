using AQMod.Localization;
using Microsoft.Xna.Framework;
using System.ComponentModel;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace AQMod
{
    [Label("$Mods.AQMod.ClientConfig.Name")]
    [BackgroundColor(10, 10, 40, 220)]
    public class AQConfigClient : ModConfig
    {
        public static AQConfigClient Instance => ModContent.GetInstance<AQConfigClient>();

        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Header(AQText.ConfigHeaderKey + "Visuals")]

        [BackgroundColor(140, 29, 47, 180)]
        [Label(AQText.ConfigValueKey + "EffectQuality")]
        [DefaultValue(1f)]
        [Range(0.1f, 2f)]
        public float EffectQuality { get; set; }
        /// <summary>
        /// Use this value to check if you should actually run unnecessary drawcode operations. Default value is 1.
        /// </summary>
        public static float c_EffectQuality { get; private set; }

        [BackgroundColor(140, 29, 47, 180)]
        [Label(AQText.ConfigValueKey + "EffectIntensity")]
        [DefaultValue(1f)]
        [Range(0.1f, 1f)]
        public float EffectIntensity { get; set; }
        /// <summary>
        /// Use this value to tune down bright, flashy things. Default value is 1.
        /// </summary>
        public static float c_EffectIntensity { get; private set; }

        [BackgroundColor(140, 29, 47, 180)]
        [Label("$" + AQText.Key + "ClientConfig.Screenshakes")]
        [DefaultValue(true)]
        public bool Screenshakes { get; set; }
        public static bool c_Screenshakes { get; private set; }

        [BackgroundColor(140, 29, 47, 180)]
        [Label("$" + AQText.Key + "ClientConfig.TonsofScreenShakes")]
        [DefaultValue(false)]
        public bool TonsofScreenShakes { get; set; }
        public static bool c_TonsofScreenShakes { get; private set; }

        [Header("$Mods.AQMod.ClientConfig.Header.Xmas")]

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

        [Header("$Mods.AQMod.ClientConfig.Header.UI")]

        [BackgroundColor(12, 12, 190, 180)]
        [Label("$Mods.AQMod.ClientConfig.MapBlipColor")]
        [Tooltip("$Mods.AQMod.ClientConfig.MapBlipColorTooltip")]
        [DefaultValue(typeof(Color), "200, 60, 145, 255"), ColorNoAlpha]
        public Color MapBlipColor { get; set; }

        [Header(AQText.ConfigHeaderKey + "Misc")]

        [BackgroundColor(150, 12, 166, 180)]
        [Label(AQText.ConfigValueKey + "CosmicEnergyAlt")]
        [DefaultValue(false)]
        public bool CosmicEnergyAlt { get; set; }
        public static bool c_CosmicEnergyAlt { get; private set; }

        [Label(AQText.ConfigValueKey + "ShowCompletedAnglerQuestsCount")]
        [BackgroundColor(12, 12, 190, 180)]
        [DefaultValue(true)]
        public bool ShowCompletedQuestsCount { get; set; }

        public override void OnChanged()
        {
            c_EffectQuality = EffectQuality;
            c_EffectIntensity = EffectIntensity;
            c_Screenshakes = Screenshakes;
            c_TonsofScreenShakes = TonsofScreenShakes;
            if (CosmicEnergyAlt != c_CosmicEnergyAlt)
            {
                c_CosmicEnergyAlt = CosmicEnergyAlt;
                Main.itemTexture[ModContent.ItemType<Items.Materials.Energies.CosmicEnergy>()] =
                    ModContent.GetTexture(ItemLoader.GetItem(ModContent.ItemType<Items.Materials.Energies.CosmicEnergy>()).Texture);
            }
        }
    }
}