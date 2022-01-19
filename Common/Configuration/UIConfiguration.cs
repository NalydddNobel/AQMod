using Microsoft.Xna.Framework;
using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace AQMod.Common.Configuration
{
    [Label("$Mods.AQMod.UIConfiguration.Name")]
    public sealed class UIConfiguration : AQConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Header("$Mods.AQMod.UIConfiguration.Header.UI")]
        [BackgroundColor(13, 166, 231, 180)]
        [Label("$Mods.AQMod.UIConfiguration.MapUITogglesPosition")]
        [Tooltip("$Mods.AQMod.UIConfiguration.MapUITogglesPositionTooltip")]
        [Range(60f, 1920f)]
        [DefaultValue(typeof(Vector2), "60, 60")]
        public Vector2 MapUITogglesPosition;
    }
}