using Microsoft.Xna.Framework;
using System.ComponentModel;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace AQMod.Common
{
    [Label("$Mods.AQMod.ClientOptions.Name")]
    [BackgroundColor(10, 10, 40, 220)]
    internal class ClientOptions : ModConfig
    {
        public static ClientOptions Instance => ModContent.GetInstance<ClientOptions>();

        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Header("$Mods.AQMod.ClientOptions.Header.Visuals")]

        [BackgroundColor(140, 29, 47, 180)]
        [Label("$Mods.AQMod.ClientOptions.FXQuality")]
        [DefaultValue(1f)]
        [Range(0.1f, 2f)]
        public float FXQuality { get; set; }

        [BackgroundColor(140, 29, 47, 180)]
        [Label("$Mods.AQMod.ClientOptions.FXIntensity")]
        [DefaultValue(1f)]
        [Range(0.1f, 1f)]
        public float FXIntensity { get; set; }

        [Header("$Mods.AQMod.ClientConfig.Header.Starite")]

        [BackgroundColor(13, 166, 231, 180)]
        [Label("$Mods.AQMod.ClientConfig.OmegaStarite3DPronunciation")]
        [Tooltip("$Mods.AQMod.ClientConfig.OmegaStarite3DPronunciationTooltip")]
        [DefaultValue(1f)]
        [Range(0f, 1.5f)]
        public float OmegaStarite3DPronunciation { get; set; }

        [BackgroundColor(13, 166, 231, 180)]
        [Label("$Mods.AQMod.ClientConfig.StaritesInTheBackground")]
        [Tooltip("$Mods.AQMod.ClientConfig.StaritesInTheBackgroundTooltip")]
        [DefaultValue(true)]
        public bool StaritesInTheBackground { get; set; }

        [BackgroundColor(13, 166, 231, 180)]
        [Label("$Mods.AQMod.ClientConfig.StariteProjectileColoring")]
        [Tooltip("$Mods.AQMod.ClientConfig.StariteProjectileColoringTooltip")]
        [DefaultValue(typeof(Color), "200, 10, 255, 0"), ColorNoAlpha]
        public Color StariteProjectileColoring { get; set; }

        [BackgroundColor(13, 166, 231, 180)]
        [Label("$Mods.AQMod.ClientConfig.StariteAuraColoring")]
        [Tooltip("$Mods.AQMod.ClientConfig.StariteAuraColoringTooltip")]
        [DefaultValue(typeof(Color), "100, 100, 255, 0"), ColorNoAlpha]
        public Color StariteAuraColoring { get; set; }

        [BackgroundColor(13, 166, 231, 180)]
        [Label("$Mods.AQMod.ClientConfig.StariteBackgroundIntensity")]
        [Tooltip("$Mods.AQMod.ClientConfig.StariteBackgroundIntensityTooltip")]
        [DefaultValue(1f)]
        [Range(0f, 1f)]
        public float StariteBackgroundIntensity { get; set; }
    }
}