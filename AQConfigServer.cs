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
        [Label("$Mods.AQMod.ServerConfig.EvilProgressionLock")]
        [Tooltip("$Mods.AQMod.ServerConfig.EvilProgressionLockTooltip")]
        [DefaultValue(true)]
        public bool evilProgressionLock;

        [BackgroundColor(13, 166, 231, 180)]
        [Label("$Mods.AQMod.ServerConfig.HarderOmegaStarite")]
        [Tooltip("$Mods.AQMod.ServerConfig.HarderOmegaStariteTooltip")]
        [DefaultValue(false)]
        [ReloadRequired()]
        public bool harderOmegaStarite;

        [BackgroundColor(13, 166, 231, 180)]
        [Label("$Mods.AQMod.ServerConfig.JellyfishNecklace")]
        [Tooltip("$Mods.AQMod.ServerConfig.JellyfishNecklaceTooltip")]
        [DefaultValue(true)]
        public bool removeJellyfishNecklace;

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
                return reduceSpawnrates();
            return false;
        }

        internal static bool reduceSpawnrates()
        {
            return NPC.AnyNPCs(ModContent.NPCType<OmegaStarite>()) || NPC.AnyNPCs(ModContent.NPCType<JerryCrabson>());
        }

        public static bool ShouldRemoveSpawns()
        {
            return ModContent.GetInstance<AQConfigServer>().reduceSpawns && reduceSpawnrates();
        }
    }
}