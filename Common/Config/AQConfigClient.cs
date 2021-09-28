using AQMod.Content.Skies;
using AQMod.Localization;
using Microsoft.Xna.Framework;
using System.ComponentModel;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace AQMod.Common.Config
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

        [Label(AQText.ConfigValueKey + "EffectIntensity")]
        [DefaultValue(1f)]
        [Range(0.1f, 2f)]
        public float EffectIntensity { get; set; }

        [Label(AQText.ConfigValueKey + "LoadItemOverlays")]
        [DefaultValue(true)]
        [ReloadRequired()]
        public bool LoadItemOverlays { get; set; }

        [Label(AQText.ConfigValueKey + "TrailShader")]
        [DefaultValue(true)]
        [ReloadRequired()]
        public bool TrailShader { get; set; }

        [Label(AQText.ConfigValueKey + "ScrollShader")]
        [DefaultValue(true)]
        [ReloadRequired()]
        public bool ScrollShader { get; set; }

        [Label(AQText.ConfigValueKey + "HypnoShader")]
        [DefaultValue(true)]
        [ReloadRequired()]
        public bool HypnoShader { get; set; }

        [Label(AQText.ConfigValueKey + "OutlineShader")]
        [DefaultValue(true)]
        [ReloadRequired()]
        public bool OutlineShader { get; set; }

        [Label(AQText.ConfigValueKey + "ScreenDistortShader")]
        [DefaultValue(true)]
        [ReloadRequired()]
        public bool ScreenDistortShader { get; set; }

        [Label(AQText.ConfigValueKey + "PortalShader")]
        [DefaultValue(true)]
        [ReloadRequired()]
        public bool PortalShader { get; set; }

        [Label(AQText.ConfigValueKey + "ColorDistortShader")]
        [DefaultValue(true)]
        [ReloadRequired()]
        public bool ColorDistortShader { get; set; }

        [Label(AQText.ConfigValueKey + "SpotlightShader")]
        [DefaultValue(true)]
        [ReloadRequired()]
        public bool SpotlightShader { get; set; }

        [Label(AQText.ConfigValueKey + "BackgroundStarites")]
        [DefaultValue(true)]
        public bool BackgroundStarites { get; set; }

        [Header(AQText.ConfigHeaderKey + "Misc")]

        [Label(AQText.ConfigValueKey + "MapBlipColor")]
        [DefaultValue(typeof(Color), "200, 60, 145, 255"), ColorHSLSlider(true), ColorNoAlpha]
        public Color MapBlipColor { get; set; }

        [Label(AQText.ConfigValueKey + "ShowCompletedAnglerQuestsCount")]
        [DefaultValue(true)]
        public bool ShowCompletedQuestsCount { get; set; }

        [Label(AQText.ConfigValueKey + "ShowCompletedRobsterQuestsCount")]
        [DefaultValue(true)]
        public bool ShowCompletedRobsterQuestsCount { get; set; }

        [Label(AQText.ConfigValueKey + "MarkerMapUIType")]
        [DrawTicks()]
        [Range(-1, 1)]
        [DefaultValue(-1)]
        public int MarkerMapUIType { get; set; }

        [Label(AQText.ConfigValueKey + "CosmicEnergyAlt")]
        [DefaultValue(false)]
        public bool CosmicEnergyAlt { get; set; }

        public override void OnChanged()
        {
            GlimmerEventSky.AssignConfig(this);
        }
    }
}