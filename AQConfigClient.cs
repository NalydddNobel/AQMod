using AQMod.Common.Skies;
using AQMod.Localization;
using Microsoft.Xna.Framework;
using System.ComponentModel;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace AQMod
{
    [Label(AQText.ConfigNameKey + "AQConfigClient")]
    public class AQConfigClient : ModConfig
    {
        public static AQConfigClient Instance => ModContent.GetInstance<AQConfigClient>();

        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Header(AQText.ConfigHeaderKey + "Visuals")]

        [Label(AQText.ConfigValueKey + "EffectQuality")]
        [DefaultValue(1f)]
        [Range(0.1f, 2f)]
        public float EffectQuality { get; set; }
        /// <summary>
        /// Use this value to check if you should actually run unnecessary drawcode operations. Default value is 1.
        /// </summary>
        public static float c_EffectQuality { get; private set; }

        [Label(AQText.ConfigValueKey + "EffectIntensity")]
        [DefaultValue(1f)]
        [Range(0.1f, 1f)]
        public float EffectIntensity { get; set; }
        /// <summary>
        /// Use this value to tune down bright, flashy things. Default value is 1.
        /// </summary>
        public static float c_EffectIntensity { get; private set; }

        [Label(AQText.ConfigValueKey + "Effect3D")]
        [Tooltip(AQText.ConfigValueKey + "Effect3DTooltip")]
        [DefaultValue(1f)]
        [Range(0f, 1.5f)]
        public float Effect3D { get; set; }
        public static float c_Effect3D { get; private set; }

        [Label("$" + AQText.Key + "ClientConfig.Screenshakes")]
        [DefaultValue(true)]
        public bool Screenshakes { get; set; }
        public static bool c_Screenshakes { get; private set; }

        [Label("$" + AQText.Key + "ClientConfig.TonsofScreenShakes")]
        [DefaultValue(false)]
        public bool TonsofScreenShakes { get; set; }
        public static bool c_TonsofScreenShakes { get; private set; }

        [Label(AQText.ConfigValueKey + "BackgroundStarites")]
        [DefaultValue(true)]
        public bool BackgroundStarites { get; set; }
        public static bool c_BackgroundStarites { get; private set; }

        [Label(AQText.ConfigValueKey + "MapBlipColor")]
        [DefaultValue(typeof(Color), "200, 60, 145, 255"), ColorNoAlpha]
        public Color MapBlipColor { get; set; }

        [Label(AQText.ConfigValueKey + "StariteProjColor")]
        [DefaultValue(typeof(Color), "200, 10, 255, 0"), ColorNoAlpha]
        public Color StariteProjColor { get; set; }

        [Label(AQText.ConfigValueKey + "StariteAuraColor")]
        [Tooltip(AQText.ConfigValueKey + "StariteAuraColorTooltip")]
        [DefaultValue(typeof(Color), "100, 100, 255, 0"), ColorNoAlpha]
        public Color StariteAuraColor { get; set; }

        [Label(AQText.ConfigValueKey + "StariteBackgroundLight")]
        [Tooltip(AQText.ConfigValueKey + "StariteBackgroundLightTooltip")]
        [DefaultValue(1f)]
        [Range(0f, 1f)]
        public float StariteBackgroundLight { get; set; }

        [Header("$" + AQText.Key + "ClientConfig.Header.Worldgen")]

        [Label("$" + AQText.Key + "ClientConfig.OverrideVanillaChestLoot")]
        [Tooltip("$" + AQText.Key + "ClientConfig.OverrideVanillaChestLootTooltip")]
        [DefaultValue(true)]
        public bool OverrideVanillaChestLoot;
        public static bool c_OverrideVanillaChestLoot { get; private set; }

        [Header(AQText.ConfigHeaderKey + "Misc")]

        [Label(AQText.ConfigValueKey + "CosmicEnergyAlt")]
        [DefaultValue(false)]
        public bool CosmicEnergyAlt { get; set; }
        public static bool c_CosmicEnergyAlt { get; private set; }

        [Label(AQText.ConfigValueKey + "ShowCompletedAnglerQuestsCount")]
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