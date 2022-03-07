using AQMod.NPCs;
using AQMod.NPCs.Bosses;
using System.ComponentModel;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace AQMod
{
    [Label("$Mods.AQMod.ServerConfig.Name")]
    [BackgroundColor(10, 10, 40, 220)]
    public class AQConfigServer : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        public static AQConfigServer Instance;

        [Header("$Mods.AQMod.ServerConfig.Header.QualityOfLife")]

        [BackgroundColor(140, 29, 47, 180)]
        [Label("$Mods.AQMod.ServerConfig.DemonSiegeDowngrades")]
        [Tooltip("$Mods.AQMod.ServerConfig.DemonSiegeDowngradesTooltip")]
        [DefaultValue(false)]
        [ReloadRequired()]
        public bool demonSiegeDowngrades;

        [BackgroundColor(140, 29, 47, 180)]
        [Label("$Mods.AQMod.ServerConfig.ReduceSpawns")]
        [Tooltip("$Mods.AQMod.ServerConfig.ReduceSpawnsTooltip")]
        [DefaultValue(false)]
        [ReloadRequired()]
        public bool reduceSpawns;

        [Header("$Mods.AQMod.ServerConfig.Header.World")]

        [BackgroundColor(13, 166, 231, 180)]
        [Label("$Mods.AQMod.ServerConfig.CooldownReforges")]
        [Tooltip("$Mods.AQMod.ServerConfig.CooldownReforgesTooltip")]
        [DefaultValue(true)]
        [ReloadRequired()]
        public bool cooldownReforges;

        [BackgroundColor(13, 166, 231, 180)]
        [Label("$Mods.AQMod.ServerConfig.JellyfishNecklace")]
        [Tooltip("$Mods.AQMod.ServerConfig.JellyfishNecklaceTooltip")]
        [DefaultValue(true)]
        public bool removeJellyfishNecklace;

        [BackgroundColor(13, 166, 231, 180)]
        [Label("$Mods.AQMod.ServerConfig.HarderOmegaStarite")]
        [Tooltip("$Mods.AQMod.ServerConfig.HarderOmegaStariteTooltip")]
        [DefaultValue(false)]
        [ReloadRequired()]
        public bool harderOmegaStarite;

        [BackgroundColor(13, 166, 231, 180)]
        [Label("$Mods.AQMod.ServerConfig.EvilProgressionLock")]
        [Tooltip("$Mods.AQMod.ServerConfig.EvilProgressionLockTooltip")]
        [DefaultValue(true)]
        public bool evilProgressionLock;

        [Header("$Mods.AQMod.ServerConfig.Header.WorldGen")]

        [BackgroundColor(195, 155, 50, 180)]
        [Label("$Mods.AQMod.ServerConfig.OverrideVanillaChestLoot")]
        [Tooltip("$Mods.AQMod.ServerConfig.OverrideVanillaChestLootTooltip")]
        [DefaultValue(true)]
        public bool overrideVanillaChestLoot;

        [BackgroundColor(195, 155, 50, 180)]
        [Label("$Mods.AQMod.ServerConfig.GenerateOceanRavines")]
        [Tooltip("$Mods.AQMod.ServerConfig.GenerateOceanRavinesTooltip")]
        [DefaultValue(true)]
        public bool generateOceanRavines;

        [BackgroundColor(195, 155, 50, 180)]
        [Label("$Mods.AQMod.ServerConfig.FixBabyPools")]
        [Tooltip("$Mods.AQMod.ServerConfig.FixBabyPoolsTooltip")]
        [DefaultValue(true)]
        public bool fixBabyPools;

        [Header("$Mods.AQMod.ServerConfig.Header.Debug")]

        [BackgroundColor(255, 193, 3, 180)]
        [Label("$Mods.AQMod.ServerConfig.DebugCommand")]
        [Tooltip("$Mods.AQMod.ServerConfig.DebugCommandTooltip")]
        [DefaultValue(false)]
        [ReloadRequired()]
        public bool debugCommand;

        public override bool NeedsReload(ModConfig pendingConfig)
        {
            if (!base.NeedsReload(pendingConfig))
            {
                return false;
            }
            var p = (AQConfigServer)pendingConfig;
            if (p.debugCommand != debugCommand ||
                p.demonSiegeDowngrades != demonSiegeDowngrades)
            {
                return true;
            }
            if (p.harderOmegaStarite != harderOmegaStarite)
                return NPC.AnyNPCs(ModContent.NPCType<OmegaStarite>());
            if (p.reduceSpawns != reduceSpawns)
                return NPCSpawns.SpawnRate_CheckBosses();
            return false;
        }
    }
}