using AQMod.Items.Accessories;
using AQMod.Items.Armor.Vanity.BossMasks;
using AQMod.Items.Placeable.CrabCrevice;
using AQMod.Items.Weapons.Magic;
using AQMod.Items.Weapons.Melee;
using AQMod.Localization;
using AQMod.NPCs;
using AQMod.NPCs.Bosses;
using System.ComponentModel;
using Terraria;
using Terraria.ID;
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
        [Label("$Mods.AQMod.ServerConfig.DemonSiegeDowngradesLabel")]
        [Tooltip("$Mods.AQMod.ServerConfig.DemonSiegeDowngradesTooltip")]
        [DefaultValue(false)]
        [ReloadRequired()]
        public bool demonSiegeDowngrades;

        [BackgroundColor(140, 29, 47, 180)]
        [Label("$Mods.AQMod.ServerConfig.ReduceSpawnsLabel")]
        [Tooltip("$Mods.AQMod.ServerConfig.ReduceSpawnsTooltip")]
        [DefaultValue(false)]
        [ReloadRequired()]
        public bool reduceSpawns;

        [Header("$Mods.AQMod.ServerConfig.Header.World")]

        [BackgroundColor(13, 166, 231, 180)]
        [Label("$Mods.AQMod.ServerConfig.CooldownReforgesLabel")]
        [Tooltip("$Mods.AQMod.ServerConfig.CooldownReforgesTooltip")]
        [DefaultValue(true)]
        [ReloadRequired()]
        public bool cooldownReforges;

        [BackgroundColor(13, 166, 231, 180)]
        [Label("$Mods.AQMod.ServerConfig.JellyfishNecklaceLabel")]
        [Tooltip("$Mods.AQMod.ServerConfig.JellyfishNecklaceTooltip")]
        [DefaultValue(true)]
        public bool removeJellyfishNecklace;

        [BackgroundColor(13, 166, 231, 180)]
        [Label("$Mods.AQMod.ServerConfig.HarderOmegaStariteLabel")]
        [Tooltip("$Mods.AQMod.ServerConfig.HarderOmegaStariteTooltip")]
        [DefaultValue(false)]
        [ReloadRequired()]
        public bool harderOmegaStarite;

        [Header("$Mods.AQMod.ServerConfig.Header.WorldGen")]

        [BackgroundColor(195, 155, 50, 180)]
        [Label("$Mods.AQMod.ServerConfig.OverrideVanillaChestLootLabel")]
        [Tooltip("$Mods.AQMod.ServerConfig.OverrideVanillaChestLootTooltip")]
        [DefaultValue(true)]
        public bool overrideVanillaChestLoot;

        [BackgroundColor(195, 155, 50, 180)]
        [Label("$Mods.AQMod.ServerConfig.GenerateOceanRavinesLabel")]
        [Tooltip("$Mods.AQMod.ServerConfig.GenerateOceanRavinesTooltip")]
        [DefaultValue(true)]
        public bool generateOceanRavines;

        [BackgroundColor(195, 155, 50, 180)]
        [Label("$Mods.AQMod.ServerConfig.FixBabyPoolsLabel")]
        [Tooltip("$Mods.AQMod.ServerConfig.FixBabyPoolsTooltip")]
        [DefaultValue(true)]
        public bool fixBabyPools;

        [Header("$Mods.AQMod.ServerConfig.Header.Debug")]

        [BackgroundColor(193, 193, 193, 180)]
        [Label("$Mods.AQMod.ServerConfig.DebugCommandLabel")]
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
                p.demonSiegeDowngrades != demonSiegeDowngrades || 
                p.cooldownReforges != cooldownReforges)
            {
                return true;
            }
            if (p.harderOmegaStarite != harderOmegaStarite)
                return NPC.AnyNPCs(ModContent.NPCType<OmegaStarite>());
            if (p.reduceSpawns != reduceSpawns)
                return NPCSpawns.SpawnRate_CheckBosses();
            return false;
        }

        public static void LoadTranslations()
        {
            AQText.AdjustTranslation("ServerConfig.DemonSiegeDowngrades", "ServerConfig.DemonSiegeDowngradesLabel", (s) => AQText.Item(ModContent.ItemType<HellsBoon>()) + " " + s);
            AQText.AdjustTranslation("ServerConfig.ReduceSpawns", "ServerConfig.ReduceSpawnsLabel", (s) => AQText.Item(ItemID.PeaceCandle) + " " + s);
            AQText.AdjustTranslation("ServerConfig.CooldownReforges", "ServerConfig.CooldownReforgesLabel", (s) => AQText.Item(ModContent.ItemType<Umystick>()) + " " + s);
            AQText.AdjustTranslation("ServerConfig.JellyfishNecklace", "ServerConfig.JellyfishNecklaceLabel", (s) => AQText.Item(ModContent.ItemType<ShockCollar>()) + " " + s);
            AQText.AdjustTranslation("ServerConfig.HarderOmegaStarite", "ServerConfig.HarderOmegaStariteLabel", (s) => AQText.Item(ModContent.ItemType<OmegaStariteMask>()) + " " + s);
            AQText.AdjustTranslation("ServerConfig.EvilProgressionLock", "ServerConfig.EvilProgressionLockLabel", (s) => AQText.Item(ItemID.ShadowOrb) + " " + s);
            AQText.AdjustTranslation("ServerConfig.OverrideVanillaChestLoot", "ServerConfig.OverrideVanillaChestLootLabel", (s) => AQText.Item(ItemID.GoldChest) + " " + s);
            AQText.AdjustTranslation("ServerConfig.GenerateOceanRavines", "ServerConfig.GenerateOceanRavinesLabel", (s) => AQText.Item(ModContent.ItemType<ExoticCoral>()) + " " + s);
            AQText.AdjustTranslation("ServerConfig.FixBabyPools", "ServerConfig.FixBabyPoolsLabel", (s) => AQText.Item(ItemID.WaterBucket) + " " + s);
            AQText.AdjustTranslation("ServerConfig.DebugCommand", "ServerConfig.DebugCommandLabel", (s) => AQText.Item(ItemID.ActuationRod) + " " + s);
        }
    }
}