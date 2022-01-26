using AQMod.Common.CrossMod;
using System.ComponentModel;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace AQMod.Common.Configuration
{
    [Label("$Mods.AQMod.DiscordRichPresenceConfig.Name")]
    [BackgroundColor(10, 40, 10, 220)]
    /// <summary>
    /// WARNING: does not load if the Discord Rich Presence mod is not enabled!
    /// </summary>
    public sealed class DiscordRichPresenceConfig : AQConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        public override bool Autoload(ref string name)
        {
            try
            {
                return ModLoader.GetMod("DiscordRP") != null;
            }
            catch
            {
                return false;
            }
        }

        [Header("$Mods.AQMod.DiscordRichPresenceConfig.Header.DRP")]

        [BackgroundColor(13, 230, 162, 180)]
        [Label("$Mods.AQMod.DiscordRichPresenceConfig.enableDiscordRichPresence")]
        [Tooltip("$Mods.AQMod.DiscordRichPresenceConfig.enableDiscordRichPresenceTooltip")]
        [DefaultValue(true)]
        public bool enableDiscordRichPresence;

        [BackgroundColor(13, 230, 162, 180)]
        [Label("$Mods.AQMod.DiscordRichPresenceConfig.miniBossRichPresence")]
        [Tooltip("$Mods.AQMod.DiscordRichPresenceConfig.miniBossRichPresenceTooltip")]
        [DefaultValue(true)]
        public bool miniBossRichPresence;

        [BackgroundColor(13, 230, 162, 180)]
        [Label("$Mods.AQMod.DiscordRichPresenceConfig.eventRichPresence")]
        [Tooltip("$Mods.AQMod.DiscordRichPresenceConfig.eventRichPresenceTooltip")]
        [DefaultValue(true)]
        public bool eventRichPresence;
    }
}