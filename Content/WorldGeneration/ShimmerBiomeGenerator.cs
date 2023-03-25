using Aequus.Items.Materials.Gems;
using System;
using System.Threading;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.WorldGeneration {
    public class ShimmerBiomeGenerator : Generator {

        public WorldGen.GrowTreeSettings[] treeProfiles = new WorldGen.GrowTreeSettings[] {
            WorldGen.GrowTreeSettings.Profiles.GemTree_Amethyst,
            WorldGen.GrowTreeSettings.Profiles.GemTree_Topaz,
            WorldGen.GrowTreeSettings.Profiles.GemTree_Sappphire,
            WorldGen.GrowTreeSettings.Profiles.GemTree_Emerald,
            WorldGen.GrowTreeSettings.Profiles.GemTree_Ruby,
            WorldGen.GrowTreeSettings.Profiles.GemTree_Diamond,
            WorldGen.GrowTreeSettings.Profiles.GemTree_Amber,
        };

        private bool CheckStone(int x, int y) {
            for (int i = -1; i < 2; i++) {
                for (int j = -1; j < 2; j++) {
                    var tile = Main.tile[x + i, y + j];
                    if (tile.IsFullySolid()) {
                        return tile.TileType == TileID.Stone || tile.TileType == TileID.Pearlstone;
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

        public void GenerateGemstones(int x, int y) {

            int tileID = ModContent.TileType<OmniGemTile>();
            int amt = Main.maxTilesX * Main.maxTilesY / 50;
            int rangeX = Main.maxTilesX / 20;
            int rangeY = Main.maxTilesY / 7;

            SetText(TextHelper.GetTextValue("WorldGeneration.OmniGem"));
            for (int i = 0; i < amt; i++) {
                int gemstoneX = Rand.Next(x - rangeX, x + rangeX);
                int gemstoneY = Rand.Next(y - rangeY, y + rangeY);

                SetProgress(i / 2f / amt);
                if (!WorldGen.InWorld(gemstoneX, gemstoneY, 5) || !Main.tile[gemstoneX, gemstoneY].HasTile || !RollPlace(x, y, rangeX, rangeY, gemstoneX, gemstoneY)) {
                    continue;
                }

                int tileType = Main.tile[gemstoneX, gemstoneY].TileType;
                if (TileID.Sets.Conversion.Sand[tileType] || TileID.Sets.Conversion.Ice[tileType] || TileID.Sets.Conversion.HardenedSand[tileType] || TileID.Sets.Conversion.Sandstone[tileType] || TileID.Sets.IcesSnow[tileType] || TileID.Sets.IcesSlush[tileType] || tileType == TileID.Silt || tileType == TileID.Mud || tileType == TileID.JungleGrass || tileType == TileID.MushroomGrass || tileType == TileID.Dirt || tileType == TileID.Grass) {
                    Main.tile[gemstoneX, gemstoneY].TileType = TileID.Stone;
                    continue;
                }
            }
            for (int i = 0; i < amt; i++) {
                int gemstoneX = Rand.Next(x - rangeX, x + rangeX);
                int gemstoneY = Rand.Next(y - rangeY, y + rangeY);

                SetProgress(i / 2f / amt + 0.5f);
                if (!WorldGen.InWorld(gemstoneX, gemstoneY, 5)
                    || Main.wallDungeon[Main.tile[gemstoneX, gemstoneY].WallType] || Main.tile[gemstoneX, gemstoneY].WallType == WallID.LihzahrdBrickUnsafe
                    || !CheckStone(gemstoneX, gemstoneY)) {
                    continue;
                }

                WorldGen.PlaceTile(gemstoneX, gemstoneY, tileID, mute: true);
                if (Main.tile[gemstoneX, gemstoneY + 1].IsFullySolid()) {
                    WorldGen.GrowTreeWithSettings(gemstoneX, gemstoneY, Rand.Next(treeProfiles));
                }

                if (Main.tile[gemstoneX, gemstoneY].HasTile) {
                    if (Main.tile[gemstoneX, gemstoneY].TileType == tileID) {
                        i += 1000;
                        continue;
                    }
                    //if (Main.tile[gemstoneX, gemstoneY].TileType == TileID.Stone) {
                    //    Main.tile[gemstoneX, gemstoneY].TileType = TileID.Pearlstone;
                    //    continue;
                    //}
                }
            }
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