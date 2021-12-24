using AQMod.Common.Skies;
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

        [Header("$" + AQText.Key + "ClientConfig.Header.Worldgen")]

        [BackgroundColor(166, 166, 12, 180)]
        [Label("$" + AQText.Key + "ClientConfig.OverrideVanillaChestLoot")]
        [Tooltip("$" + AQText.Key + "ClientConfig.OverrideVanillaChestLootTooltip")]
        [DefaultValue(true)]
        public bool OverrideVanillaChestLoot { get; private set; }
        public static bool c_OverrideVanillaChestLoot { get; private set; }

        [Header("$Mods.AQMod.ClientConfig.Header.Starite")]

        [BackgroundColor(13, 166, 231, 180)]
        [Label("$Mods.AQMod.ClientConfig.Effect3D")]
        [Tooltip("$Mods.AQMod.ClientConfig.Effect3DTooltip")]
        [DefaultValue(1f)]
        [Range(0f, 1.5f)]
        public float Effect3D { get; set; }
        public static float c_Effect3D { get; private set; }

        [BackgroundColor(13, 166, 231, 180)]
        [Label("$Mods.AQMod.ClientConfig.BackgroundStarites")]
        [Tooltip("$Mods.AQMod.ClientConfig.BackgroundStaritesTooltip")]
        [DefaultValue(true)]
        public bool BackgroundStarites { get; set; }
        public static bool c_BackgroundStarites { get; private set; }

        [BackgroundColor(13, 166, 231, 180)]
        [Label("$Mods.AQMod.ClientConfig.StariteProjColor")]
        [Tooltip("$Mods.AQMod.ClientConfig.StariteProjColorTooltip")]
        [DefaultValue(typeof(Color), "200, 10, 255, 0"), ColorNoAlpha]
        public Color StariteProjColor { get; set; }

        [BackgroundColor(13, 166, 231, 180)]
        [Label("$Mods.AQMod.ClientConfig.StariteAuraColor")]
        [Tooltip("$Mods.AQMod.ClientConfig.StariteAuraColorTooltip")]
        [DefaultValue(typeof(Color), "100, 100, 255, 0"), ColorNoAlpha]
        public Color StariteAuraColor { get; set; }

        [BackgroundColor(13, 166, 231, 180)]
        [Label("$Mods.AQMod.ClientConfig.StariteBackgroundLight")]
        [Tooltip("$Mods.AQMod.ClientConfig.StariteBackgroundLightTooltip")]
        [DefaultValue(1f)]
        [Range(0f, 1f)]
        public float StariteBackgroundLight { get; set; }

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

        public override bool AcceptClientChanges(ModConfig pendingConfig, int whoAmI, ref string message)
        {
            var client = (AQConfigClient)pendingConfig;
            if (client.OverrideVanillaChestLoot != OverrideVanillaChestLoot)
            {
                return Main.gameMenu;
            }
            return base.AcceptClientChanges(pendingConfig, whoAmI, ref message);
        }

        public override void OnChanged()
        {
            c_EffectQuality = EffectQuality;
            c_EffectIntensity = EffectIntensity;
            c_Effect3D = Effect3D;
            c_Screenshakes = Screenshakes;
            c_TonsofScreenShakes = TonsofScreenShakes;
            c_BackgroundStarites = BackgroundStarites;
            if (c_BackgroundStarites != BackgroundStarites)
            {
                c_BackgroundStarites = BackgroundStarites;
                GlimmerEventSky._starites = null;
                GlimmerEventSky._lonelyStarite = null;
            }
            if (CosmicEnergyAlt != c_CosmicEnergyAlt)
            {
                c_CosmicEnergyAlt = CosmicEnergyAlt;
                Main.itemTexture[ModContent.ItemType<Items.Materials.Energies.CosmicEnergy>()] =
                    ModContent.GetTexture(ItemLoader.GetItem(ModContent.ItemType<Items.Materials.Energies.CosmicEnergy>()).Texture);
            }
            AQMod.ApplyClientConfig(this);
        }
    }
}