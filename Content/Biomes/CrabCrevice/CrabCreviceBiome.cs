﻿#if !CRAB_CREVICE_DISABLE
using Aequus.Common;
using Aequus.Common.Items.DropRules;
using Aequus.Common.Preferences;
using Aequus.Content.Biomes.CrabCrevice.Background;
using Aequus.Content.Biomes.CrabCrevice.Water;
using Aequus.Items.Equipment.Accessories.Combat.OnHitAbility.Anchor;
using Aequus.Items.Equipment.Accessories.Sentry.SentrySquid;
using Aequus.Items.Equipment.Accessories.Water;
using Aequus.Items.Weapons.Ranged.Misc.StarPhish;
using Aequus.NPCs.Monsters.CrabCrevice;
using Aequus.Tiles.CrabCrevice;
using System.Collections.Generic;

namespace Aequus.Content.Biomes.CrabCrevice;
public class CrabCreviceBiome : ModBiome {
    public static ConfiguredMusicData music { get; private set; }

    public override string BestiaryIcon => AequusTextures.CrabCreviceBestiaryIcon.FullPath;

    public override string BackgroundPath => Aequus.VanillaTexture + "MapBG11";
    public override string MapBackground => BackgroundPath;

    public override ModWaterStyle WaterStyle => ModContent.GetInstance<CrabCreviceWater>();
    public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<CrabCreviceSurfaceBackground>();
    public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.GetInstance<CrabCreviceUGBackground>();

    public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;

    public override int Music => music.GetID();

    public override void Load() {
        if (!Main.dedServ) {
            music = new ConfiguredMusicData(MusicID.OceanNight, MusicID.OtherworldlyOcean);
        }
    }

    public override void SetStaticDefaults() {
        SetChestLoot();
    }

    public override void Unload() {
        UnloadChestLoot();
        music = null;
    }

    public override bool IsBiomeActive(Player player) {
        if (SedimentaryRockTile.BiomeCount > 150 || (Main.remixWorld && player.ZoneUndergroundDesert))
            return true;

        var loc = player.Center.ToTileCoordinates();
        return WorldGen.InWorld(loc.X, loc.Y, 10) && Main.tile[loc].WallType == ModContent.WallType<SedimentaryRockWallPlaced>();
    }

    internal static bool SpawnCrabCreviceEnemies(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo) {
        var spawnTile = Main.tile[spawnInfo.SpawnTileX, spawnInfo.SpawnTileY];
        var aboveTile = Main.tile[spawnInfo.SpawnTileX, spawnInfo.SpawnTileY - 1];
        if (!spawnInfo.Player.InModBiome<CrabCreviceBiome>()
            && spawnTile.WallType != ModContent.WallType<SedimentaryRockWallPlaced>()) {
            return false;
        }

        pool.Clear();
        if (Aequus.MediumMode) {
            pool[ModContent.NPCType<SummonerCrab>()] = 0.2f;
        }
        pool.Add(NPCID.SeaSnail, 0.08f);

        pool.Add(ModContent.NPCType<SoldierCrab>(), 0.33f);
        pool.Add(ModContent.NPCType<CoconutCrab>(), 0.33f);
        pool.Add(NPCID.Crab, 0.33f);

        if (spawnInfo.Water || Main.remixWorld) {
            if (!NPC.AnyNPCs(ModContent.NPCType<CrabFish>()))
                pool.Add(ModContent.NPCType<CrabFish>(), 0.33f);
            pool.Add(NPCID.Shark, 0.08f);

            pool.Add(NPCID.Seahorse, 0.05f);

            if (GameplayConfig.Instance.EarlyGreenJellyfish)
                pool.Add(NPCID.GreenJellyfish, 0.33f);
            if (GameplayConfig.Instance.EarlyAnglerFish)
                pool.Add(NPCID.AnglerFish, 0.33f);
        }
        return true;
    }

    #region Chest Contents
    /// <summary>
    /// The primary loot pool for Crab Crevice chests
    /// </summary>
    public static ItemDrop[] ChestPrimaryLoot;
    /// <summary>
    /// The secondary loot pool for Crab Crevice chests
    /// </summary>
    public static ItemDrop[] ChestSecondaryLoot;
    /// <summary>
    /// The tertiary (3rd) loot pool for Crab Crevice chests
    /// </summary>
    public static ItemDrop[] ChestTertiaryLoot;

    public void SetChestLoot() {
        ChestPrimaryLoot = new ItemDrop[]
        {
            ModContent.ItemType<StarPhish>(),
            ModContent.ItemType<DavyJonesAnchor>(),
            ModContent.ItemType<BreathConserver>(),
            ModContent.ItemType<SentrySquid>(),
        };

        ChestSecondaryLoot = new ItemDrop[]
        {
            ItemID.Trident,
            ItemID.FloatingTube,
            ItemID.Flipper,
            ItemID.WaterWalkingBoots,
            ItemID.BreathingReed,
        };

        ChestTertiaryLoot = new ItemDrop[]
        {
            ItemID.DivingHelmet,
            ItemID.BeachBall,
            ItemID.JellyfishNecklace,
        };
    }

    public void UnloadChestLoot() {
        ChestTertiaryLoot = null;
        ChestSecondaryLoot = null;
        ChestPrimaryLoot = null;
    }
    #endregion
}
#endif