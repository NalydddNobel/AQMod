using Aequus.Content.World.Generation;
using Aequus.Items.Materials.Gems;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Biomes.Aether {
    public class LegacyOmniGemsGenerator : Generator {
        public static readonly HashSet<ushort> ConvertibleTiles = new();
        public static readonly List<WorldGen.GrowTreeSettings> treeProfiles = new();

        public override void SetStaticDefaults() {
            treeProfiles.Add(WorldGen.GrowTreeSettings.Profiles.GemTree_Amethyst);
            treeProfiles.Add(WorldGen.GrowTreeSettings.Profiles.GemTree_Topaz);
            treeProfiles.Add(WorldGen.GrowTreeSettings.Profiles.GemTree_Sappphire);
            treeProfiles.Add(WorldGen.GrowTreeSettings.Profiles.GemTree_Emerald);
            treeProfiles.Add(WorldGen.GrowTreeSettings.Profiles.GemTree_Ruby);
            treeProfiles.Add(WorldGen.GrowTreeSettings.Profiles.GemTree_Diamond);
            treeProfiles.Add(WorldGen.GrowTreeSettings.Profiles.GemTree_Amber);
            ConvertibleTiles.Add(TileID.Silt);
            ConvertibleTiles.Add(TileID.Mud);
            ConvertibleTiles.Add(TileID.JungleGrass);
            ConvertibleTiles.Add(TileID.MushroomGrass);
            ConvertibleTiles.Add(TileID.Dirt);
            ConvertibleTiles.Add(TileID.Grass);
        }

        public override void Unload() {
            treeProfiles.Clear();
            ConvertibleTiles.Clear();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsTileConvertible(Tile tile) {
            ushort tileType = tile.TileType;
            return TileID.Sets.Conversion.Sand[tileType] || TileID.Sets.Conversion.Ice[tileType] || TileID.Sets.Conversion.HardenedSand[tileType] || TileID.Sets.Conversion.Sandstone[tileType] || TileID.Sets.IcesSnow[tileType] || TileID.Sets.IcesSlush[tileType] || ConvertibleTiles.Contains(tileType);
        }

        private bool ValidGemPlacement(int x, int y) {

            var tile = Main.tile[x, y];
            int wall = tile.WallType;
            if (Main.wallDungeon[wall] || Main.wallHouse[wall] || wall == WallID.LihzahrdBrickUnsafe) {
                return false;
            }

            for (int i = -1; i < 2; i++) {
                for (int j = -1; j < 2; j++) {
                    var nearbyTile = Main.tile[x + i, y + j];
                    if (nearbyTile.IsFullySolid()) {
                        return nearbyTile.TileType == TileID.Stone || nearbyTile.TileType == TileID.Pearlstone;
                    }
                }
            }

            return false;
        }

        private bool RollPlace(int x, int y, int w, int h, int gemstoneX, int gemstoneY) {

            float distance = MathF.Pow((Math.Abs(gemstoneX - x) + Math.Abs(gemstoneY - y)) / (float)(w + h), 3f);
            int chance = Math.Max((int)(distance * 50), 1);
            return Rand.NextBool(chance);
        }

        public void SpreadStone(int x, int y, int rangeX, int rangeY, int amt) {
            for (int i = 0; i < amt; i++) {
                int gemstoneX = Rand.Next(x - rangeX, x + rangeX);
                int gemstoneY = Rand.Next(y - rangeY, y + rangeY);

                SetProgress(i / 2f / amt);
                if (!WorldGen.InWorld(gemstoneX, gemstoneY, 5) || !Main.tile[gemstoneX, gemstoneY].HasTile || !RollPlace(x, y, rangeX, rangeY, gemstoneX, gemstoneY)) {
                    continue;
                }

                int tileType = Main.tile[gemstoneX, gemstoneY].TileType;
                if (IsTileConvertible(Main.tile[gemstoneX, gemstoneY])) {
                    Main.tile[gemstoneX, gemstoneY].TileType = TileID.Stone;
                    continue;
                }
            }
        }

        public void GrowGems(int x, int y, int rangeX, int rangeY, int amt) {

            int omniGemTileID = ModContent.TileType<OmniGemTile>();
            for (int i = 0; i < amt; i++) {
                int gemstoneX = Rand.Next(x - rangeX, x + rangeX);
                int gemstoneY = Rand.Next(y - rangeY, y + rangeY);

                SetProgress(i / 2f / amt + 0.5f);
                if (!WorldGen.InWorld(gemstoneX, gemstoneY, 5) || !ValidGemPlacement(gemstoneX, gemstoneY)) {
                    continue;
                }

                if (Main.tile[gemstoneX, gemstoneY + 1].IsFullySolid()) {
                    WorldGen.GrowTreeWithSettings(gemstoneX, gemstoneY, Rand.Next(treeProfiles));
                }
                else {
                    WorldGen.PlaceTile(gemstoneX, gemstoneY, omniGemTileID, mute: true);
                }

                if (Main.tile[gemstoneX, gemstoneY].HasTile) {
                    if (Main.tile[gemstoneX, gemstoneY].TileType == omniGemTileID) {
                        i += 1000;
                        continue;
                    }
                }
            }
        }

        public void GenerateGemstones(int x, int y) {

            int amt = Main.maxTilesX * Main.maxTilesY / 50;
            int rangeX = Main.maxTilesX / 20;
            int rangeY = Main.maxTilesY / 7;

            SetText("WorldGeneration.OmniGem");
            SpreadStone(x, y, rangeX, rangeY, amt);
            GrowGems(x, y, rangeX, rangeY, amt);
            SetProgress(1f);
        }

        protected override void Generate() {

            int x = Rand.Next(100, Main.maxTilesX / 4);
            if (Main.dungeonX * 2 < Main.maxTilesX) {
                x = Main.maxTilesX - x;
            }
            int y = Rand.Next((int)Main.rockLayer + 50, (int)Main.rockLayer + 150);

            GenerateGemstones(x, y);
        }
    }
}