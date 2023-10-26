using Aequus.Common.Tiles;
using Aequus.Content.WorldGeneration;
using Aequus.Core.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.WorldBuilding;

namespace Aequus.Common.WorldGeneration;

public class AequusWorldGenerationSystem : ModSystem {
    public static List<AequusGenStep> GenerationSteps = new();

    public override void Unload() {
        GenerationSteps.Clear();
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
        var placedItems = new HashSet<int>();
        var r = WorldGen.genRand;

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

            if (tile.TileType == TileID.Containers) {
                if (style == ChestType.Gold) {
                    PostGenerationSteps.CheckUGGoldChest(chest);
                }
                if (style == ChestType.LockedShadow) {
                    PostGenerationSteps.CheckShadowChest(chest);
                }
                continue;
            }

            if (Main.tile[chest.x, chest.y].TileType == TileID.Containers2) {
                continue;
            }
        }
    }
}