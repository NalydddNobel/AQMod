using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace AQMod.Common.Configuration
{
    [Label("$Mods.AQMod.WorldGenOptions.Name")]
    [BackgroundColor(20, 20, 13, 180)]
    public sealed class WorldGenOptions : AQConfig
    {
        // Most Worldgen is done clientside I think,
        // but this config could also be used for Hardmode structures?
        public override ConfigScope Mode => ConfigScope.ServerSide; 

        [Header("$Mods.AQMod.WorldGenOptions.Header.OnWorldCreation")]

        [BackgroundColor(110, 100, 85, 180)]
        [Label("$Mods.AQMod.WorldGenOptions.fixBabyPools")]
        [Tooltip("$Mods.AQMod.WorldGenOptions.fixBabyPoolsTooltip")]
        [DefaultValue(true)]
        public bool fixBabyPools;

        [BackgroundColor(110, 100, 85, 180)]
        [Label("$Mods.AQMod.WorldGenOptions.generateOceanRavines")]
        [Tooltip("$Mods.AQMod.WorldGenOptions.generateOceanRavinesTooltip")]
        [DefaultValue(true)]
        public bool generateOceanRavines;

        [Header("$Mods.AQMod.WorldGenOptions.Header.ChestLoot")]

        [BackgroundColor(166, 130, 12, 180)]
        [Label("$Mods.AQMod.WorldGenOptions.overrideVanillaChestLoot")]
        [Tooltip("$Mods.AQMod.WorldGenOptions.overrideVanillaChestLootTooltip")]
        [DefaultValue(true)]
        public bool overrideVanillaChestLoot;
    }
}