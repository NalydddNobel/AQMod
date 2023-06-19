using Aequus.Common.GlobalTiles;
using Aequus.Content.Biomes.MossBiomes.Tiles.ElitePlants;
using Aequus.Items.Materials.Gems;
using Aequus.Items.Materials.PearlShards;
using Aequus.Items.Weapons.Melee.BattleAxe;
using Aequus.Tiles.CrabCrevice;
using Aequus.Tiles.Misc.Herbs.Manacle;
using Aequus.Tiles.Misc.Herbs.Mistral;
using Aequus.Tiles.Misc.Herbs.Moonflower;
using FullSerializer.Internal;
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

        private static void WorldGen_UpdateWorld_OvergroundTile(On_WorldGen.orig_UpdateWorld_OvergroundTile orig, int i, int j, bool checkNPCSpawns, int wallDist) {
            if (!DisableRandomTickUpdate(i, j)) {
                return;
            }
            orig(i, j, checkNPCSpawns, wallDist);
        }

        private static void SurfaceRandomUpdate(in GlobalRandomTileUpdateParams info) {
            // Ignore most floating islands
            if (info.GemY > 250) {
                if (WorldGen.genRand.NextBool(1200)) {
                    if (Aequus.AnyBossDefeated) {
                        BattleAxeTile.TrySpawnBattleAxe(in info);
                    }
                }
            }
        }
        private static void UndergroundRandomUpdate(in GlobalRandomTileUpdateParams info) {
            ElitePlantTile.GlobalRandomUpdate(in info);
            if (!Main.remixWorld) {
                if (info.TileTypeCache == TileID.Vines || info.TileTypeCache == TileID.VineFlowers) {
                    Helper.iterations++;
                    AequusWorld.RandomUpdateTile_Surface(info.X, info.Y, checkNPCSpawns: false, wallDist: 3);
                    Helper.iterations--;
                }
                if (info.TileTypeCache == TileID.Grass && info.Tile.Slope != SlopeType.SlopeUpLeft && info.Tile.Slope != SlopeType.SlopeUpRight && !Main.tile[info.X, info.Y + 1].HasTile && WorldGen.genRand.NextBool(10)) {
                    WorldGen.PlaceTile(info.X, info.Y + 1, WorldGen.genRand.NextBool(4) ? TileID.VineFlowers : TileID.Vines, mute: true);
                    if (Main.tile[info.X, info.Y + 1].HasTile && Main.netMode != NetmodeID.SinglePlayer) {
                        NetMessage.SendTileSquare(-1, info.X, info.Y + 1);
                    }
                }
            }
            if ((TileID.Sets.Stone[info.TileTypeCache] || Main.tileMoss[info.TileTypeCache]) && WorldGen.genRand.NextBool(10)) {
                OmniGemTile.TryGrow(in info);
            }
        }

        public override void RandomUpdate(int i, int j, int type) {
            if (Helper.iterations != 0) {
                return;
            }
            GlobalRandomTileUpdateParams info = new(i, j, type, Main.tile[i, j].WallType);

            if (j <= (int)Main.worldSurface) {
                SurfaceRandomUpdate(in info);
                MistralTile.GlobalRandomUpdate(i, j);
                MoonflowerTile.GlobalRandomUpdate(i, j);
            }
            else {
                UndergroundRandomUpdate(in info);
                ManacleTile.GlobalRandomUpdate(i, j);
            }
        }
    }
}