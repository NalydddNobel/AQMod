using AQMod.Localization;
using System.ComponentModel;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace AQMod.Common.Config
{
    [Label(AQText.ConfigNameKey + "AQConfigServer")]
    public class AQConfigServer : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        public static AQConfigServer Instance => ModContent.GetInstance<AQConfigServer>();

        [Header(AQText.ConfigHeaderKey + "World")]

        [Label(AQText.ConfigValueKey + "EvilProgressionLock")]
        [Tooltip(AQText.ConfigValueKey + "EvilProgressionLockTooltip")]
        [DefaultValue(true)]
        [ReloadRequired()]
        public bool evilProgressionLock;

        [Header(AQText.ConfigHeaderKey + "Debug")]

        [Label(AQText.ConfigValueKey + "AprilFools")]
        [DefaultValue(false)]
        [ReloadRequired()]
        public bool aprilFools;

        [Label(AQText.ConfigValueKey + "DebugCommand")]
        [DefaultValue(false)]
        [ReloadRequired()]
        public bool debugCommand;

        public override void OnLoaded()
        {
            if (!AQMod.AprilFools)
                AQMod.AprilFools = aprilFools;
        }
    }
}