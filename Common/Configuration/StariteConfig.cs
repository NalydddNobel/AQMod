using AQMod.Content.World.Events.GlimmerEvent;
using Microsoft.Xna.Framework;
using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace AQMod.Common.Configuration
{
    public sealed class StariteConfig : AQConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Header("$Mods.AQMod.ClientConfig.Header.Starite")]

        [BackgroundColor(13, 166, 231, 180)]
        [Label("$Mods.AQMod.ClientConfig.Effect3D")]
        [Tooltip("$Mods.AQMod.ClientConfig.Effect3DTooltip")]
        [DefaultValue(1f)]
        [Range(0f, 1.5f)]
        public float Effect3D { get; set; }

        [BackgroundColor(13, 166, 231, 180)]
        [Label("$Mods.AQMod.ClientConfig.BackgroundStarites")]
        [Tooltip("$Mods.AQMod.ClientConfig.BackgroundStaritesTooltip")]
        [DefaultValue(true)]
        public bool BackgroundStarites { get; set; }

        [BackgroundColor(13, 166, 231, 180)]
        [Label("$Mods.AQMod.StariteConfig.BackgroundStars")]
        [Tooltip("$Mods.AQMod.StariteConfig.BackgroundStarsTooltip")]
        [DefaultValue(true)]
        public float BackgroundStars { get; set; }

        [BackgroundColor(13, 166, 231, 180)]
        [Label("$Mods.AQMod.StariteConfig.BackgroundShader")]
        [Tooltip("$Mods.AQMod.StariteConfig.BackgroundShaderTooltip")]
        [DefaultValue(true)]
        public float BackgroundShader { get; set; }

        [BackgroundColor(13, 166, 231, 180)]
        [Label("$Mods.AQMod.StariteConfig.UltimateSwordVignette")]
        [Tooltip("$Mods.AQMod.StariteConfig.UltimateSwordVignetteTooltip")]
        [DefaultValue(true)]
        public bool UltimateSwordVignette { get; set; }

        [BackgroundColor(13, 166, 231, 180)]
        [Label("$Mods.AQMod.ClientConfig.StariteProjColor")]
        [Tooltip("$Mods.AQMod.ClientConfig.StariteProjColorTooltip")]
        [DefaultValue(typeof(Color), "200, 10, 255, 0"), ColorNoAlpha]
        public Color StariteProjectileColoring { get; set; }

        [BackgroundColor(13, 166, 231, 180)]
        [Label("$Mods.AQMod.ClientConfig.StariteAuraColor")]
        [Tooltip("$Mods.AQMod.ClientConfig.StariteAuraColorTooltip")]
        [DefaultValue(typeof(Color), "100, 100, 255, 0"), ColorNoAlpha]
        public Color AuraColoring { get; set; }

        [BackgroundColor(13, 166, 231, 180)]
        [Label("$Mods.AQMod.ClientConfig.StariteBackgroundLight")]
        [Tooltip("$Mods.AQMod.ClientConfig.StariteBackgroundLightTooltip")]
        [DefaultValue(1f)]
        [Range(0f, 1f)]
        public float BackgroundBrightness { get; set; }

        public override void OnChanged()
        {
            base.OnChanged();
            GlimmerEventSky._starites = null;
            GlimmerEventSky._lonelyStarite = null;
        }
    }
}