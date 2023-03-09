using Aequus.Common.ItemDrops;
using Aequus.Content.Biomes.CrabCrevice.Background;
using Aequus.Content.Biomes.CrabCrevice.Water;
using Aequus.Items.Accessories.Offense;
using Aequus.Items.Accessories.Utility;
using Aequus.Items.Placeable.Furniture.CraftingStation;
using Aequus.Items.Weapons.Ranged;
using Aequus.Tiles.CrabCrevice;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Biomes.CrabCrevice
{
    public class CrabCreviceBiome : ModBiome
    {
        public static ConfiguredMusicData music { get; private set; }

        public override string BestiaryIcon => "Aequus/Assets/UI/BestiaryIcons/CrabCrevice";

        public override string BackgroundPath => Aequus.VanillaTexture + "MapBG11";
        public override string MapBackground => BackgroundPath;

        public override ModWaterStyle WaterStyle => ModContent.GetInstance<CrabCreviceWater>();
        public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<CrabCreviceSurfaceBackground>();
        public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.GetInstance<CrabCreviceUGBackground>();

        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;

        public override int Music => music.GetID();

        public override void Load()
        {
            if (!Main.dedServ)
            {
                music = new ConfiguredMusicData(MusicID.OceanNight, MusicID.OtherworldlyOcean);
            }
        }

        public override void SetStaticDefaults()
        {
            SetChestLoot();
        }

        public override void Unload()
        {
            UnloadChestLoot();
            music = null;
        }

        public override bool IsBiomeActive(Player player)
        {
            if (SedimentaryRockTile.BiomeCount > 150)
                return true;

            var loc = player.Center.ToTileCoordinates();
            return WorldGen.InWorld(loc.X, loc.Y, 10) && Main.tile[loc].WallType == ModContent.WallType<SedimentaryRockWallWall>();
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

        public void SetChestLoot()
        {
            ChestPrimaryLoot = new ItemDrop[]
            {
                ModContent.ItemType<StarPhish>(),
                ModContent.ItemType<DavyJonesAnchor>(),
                ModContent.ItemType<ArmFloaties>(),
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
                ModContent.ItemType<RecyclingMachine>(),
                ItemID.DivingHelmet,
                ItemID.BeachBall,
                ItemID.JellyfishNecklace,
            };
        }

        public void UnloadChestLoot()
        {
            ChestTertiaryLoot = null;
            ChestSecondaryLoot = null;
            ChestPrimaryLoot = null;
        }
        #endregion
    }
}