using Aequus.Common.Tiles.Global;
using Aequus.Content.Items.Materials.OmniGem;
using Aequus.Tiles.Herbs.Manacle;
using Aequus.Tiles.Herbs.Mistral;
using Aequus.Tiles.Herbs.Moonflower;
using Aequus.Tiles.Misc;
using Aequus.Tiles.MossCaves.ElitePlants;
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

        public override void RandomUpdate(int i, int j, int type) {
            if (Helper.iterations != 0) {
                return;
            }
            GlobalRandomTileUpdateParams info = new(i, j, type, Main.tile[i, j].WallType);

            if (Main.remixWorld ? WorldGen.genRand.NextBool() : j <= Main.worldSurface) {
                if (!Helper.ZoneSkyHeight(info.GemY)) {
                    if (Main.dayTime) {
                        if (Aequus.AnyBossDefeated && WorldGen.genRand.NextBool(BattleAxeTile.spawnChance)) {
                            BattleAxeTile.TrySpawn(in info);
                        }
                        if (Main.hardMode && NPC.downedMechBossAny && WorldGen.genRand.NextBool(AloeVeraTile.spawnChance)) {
                            AloeVeraTile.TrySpawn(in info);
                        }
                    }
                }
                MistralTile.GlobalRandomUpdate(i, j);
                MoonflowerTile.GlobalRandomUpdate(i, j);
            }
            else {
                if (WorldGen.genRand.NextBool(ElitePlantTile.spawnChance)) {
                    ElitePlantTile.GlobalRandomUpdate(in info);
                }
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
                if ((TileID.Sets.Stone[info.TileTypeCache] || Main.tileMoss[info.TileTypeCache]) && WorldGen.genRand.NextBool(500)) {
                    OmniGemTile.TryGrow(in info);
                }
                ManacleTile.GlobalRandomUpdate(i, j);
            }
        }
    }
}