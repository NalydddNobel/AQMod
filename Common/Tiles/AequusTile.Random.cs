using Aequus.Content.Biomes.PollutedOcean.Tiles.FloatingTrash;
using Aequus.Content.Items.Material.OmniGem;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Common.Tiles {
    public partial class AequusTile {
        public static readonly HashSet<int> NoVanillaRandomTickUpdates = new();

        private static bool DisableRandomTickUpdate(int i, int j) {
            var tile = Framing.GetTileSafely(i, j);
            if (tile.HasTile && NoVanillaRandomTickUpdates.Contains(tile.TileType)) {
                TileLoader.RandomUpdate(i, j, Main.tile[i, j].TileType);
                WallLoader.RandomUpdate(i, j, Main.tile[i, j].WallType);
                return false;
            }
            return true;
        }

        private static void WorldGen_UpdateWorld_UndergroundTile(On_WorldGen.orig_UpdateWorld_UndergroundTile orig, int i, int j, bool checkNPCSpawns, int wallDist) {
            if (!DisableRandomTickUpdate(i, j)) {
                return;
            }
            orig(i, j, checkNPCSpawns, wallDist);
        }

        private static void WorldGen_UpdateWorld_OvergroundTile(On_WorldGen.orig_UpdateWorld_OvergroundTile orig, int i, int j, bool checkNPCSpawns, int wallDist) {
            if (!DisableRandomTickUpdate(i, j)) {
                return;
            }
            orig(i, j, checkNPCSpawns, wallDist);
        }

        public override void RandomUpdate(int i, int j, int type) {
            if (Main.hardMode) {
                OmniGemTile.Grow(i, j);
            }
            if (Main.dayTime && WorldGen.oceanDepths(i, j) && Main.tile[i, j - 1].LiquidAmount > 0 && TileHelper.ScanUp(new(i, j - 1), 100, out var result, TileHelper.HasNoLiquid)) {
                WorldGen.PlaceTile(result.X, result.Y + 1, ModContent.TileType<FloatingTrashTile>(), mute: true);
            }
        }
    }
}