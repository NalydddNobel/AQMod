using Aequus.Common.Tiles;
using Aequus.Content.WorldGeneration;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.WorldBuilding;

namespace Aequus.Common.WorldGeneration;

public class WorldGenSystem : ModSystem {
    public static List<AequusGenStep> GenerationSteps = new();

    public static readonly HashSet<int> PlacedItems = new();

    public override void Unload() {
        GenerationSteps.Clear();
    }

    public override void PreWorldGen() {
        PlacedItems.Clear();
    }

    public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight) {
        foreach (var step in GenerationSteps) {
            string sortAfter = step.InsertAfter;
            int index = tasks.FindIndex((pass) => pass.Name.Equals(sortAfter));
            if (index == -1) {
                step.EmergencyOnStepNotFound(tasks);
            }
            else {
                step.InsertStep(index + 1, tasks);
            }
        }
    }

    public override void PostWorldGen() {
        // A hashset of items which have been placed atleast once.
        var rand = WorldGen.genRand;

        for (int k = 0; k < Main.maxChests; k++) {
            Chest chest = Main.chest[k];
            if (chest == null || !WorldGen.InWorld(chest.x, chest.y, 40)) {
                continue;
            }

            var tile = Main.tile[chest.x, chest.y];
            var wallId = tile.WallType;
            var tileObjectData = TileObjectData.GetTileData(tile);
            int style = TileHelper.GetStyle(tile, tileObjectData, coordinateFullWidthBackup: 36);

            // Pyramid
            if (wallId == WallID.SandstoneBrick) {
                continue;
            }

            // Dungeon
            if (Main.wallDungeon[wallId]) {
                continue;
            }

            var type = tile.TileType;
            if (chest.y < Main.worldSurface) {
                if (IsChest(style, type, ChestType.Wood)) {
                    PostGenerationSteps.CheckSurfaceChest(chest);
                }
            }
            else if (chest.y < Main.UnderworldLayer) {
                if (IsChest(style, type, ChestType.Gold)) {
                    PostGenerationSteps.CheckUGGoldChest(chest);
                }
            }
            else {
                if (IsChest(style, type, ChestType.LockedShadow)) {
                    PostGenerationSteps.CheckShadowChest(chest);
                }
            }
        }


        static bool IsChest(int chestStyle, int chestTileId, int wantedStyle, int wantedTileId = TileID.Containers) 
            => wantedStyle == chestStyle && wantedTileId == chestTileId;
    }
}