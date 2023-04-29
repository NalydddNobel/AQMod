using Aequus.Content.Biomes.CrabCrevice.Tiles;
using Aequus.Content.Biomes.MossBiomes.Tiles.ElitePlants;
using Aequus.Items.Materials.Gems;
using Aequus.Items.Materials.PearlShards;
using Aequus.Tiles.Ambience;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus {
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

        private static void WorldGen_UpdateWorld_OvergroundTile(Terraria.On_WorldGen.orig_UpdateWorld_OvergroundTile orig, int i, int j, bool checkNPCSpawns, int wallDist) {
            if (!DisableRandomTickUpdate(i, j)) {
                return;
            }
            orig(i, j, checkNPCSpawns, wallDist);
        }

        public override void RandomUpdate(int i, int j, int type) {
            if (Helper.iterations != 0) {
                return;
            }

            ManacleTile.GlobalRandomUpdate(i, j, type);
            MistralTile.GlobalRandomUpdate(i, j, type);
            MoonflowerTile.GlobalRandomUpdate(i, j, type);
            if (Main.tile[i, j].WallType == ModContent.WallType<SedimentaryRockWallWall>()) {
                MorayTile.GlobalRandomUpdate(i, j, type);
                PearlsTile.GrowPearl(i, j);
            }

            if (j <= (int)Main.worldSurface) {
                return;
            }

            ElitePlantTile.GlobalRandomUpdate(i, j, type);
            if (type == TileID.Vines || type == TileID.VineFlowers) {
                Helper.iterations++;
                AequusWorld.RandomUpdateTile_Surface(i, j, checkNPCSpawns: false, wallDist: 3);
                Helper.iterations--;
            }
            if (!Main.tile[i, j + 1].HasTile && WorldGen.genRand.NextBool(30)) {
                WorldGen.PlaceTile(i, j + 1, WorldGen.genRand.NextBool(4) ? TileID.VineFlowers : TileID.Vines);
            }
            OmniGemTile.TryGrow(i, j, 60, 100);
        }
    }
}