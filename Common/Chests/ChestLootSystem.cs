using Aequus.Content.Chests;
using Aequus.DataSets;
using Terraria.ObjectData;

namespace Aequus.Common.Chests;

public class ChestLootSystem : ModSystem {
    private static bool CanAddLoot(Chest chest) {
        for (int m = 0; m < Chest.maxItems; m++) {
            if (chest.item[m] == null || chest.item[m].IsAir) {
                continue;
            }

            // Prevent Aequus from adjusting chest loot at all if it contains an important item
            if (ItemDataSet.ImportantItem.Contains(chest.item[m].type)) {
                return false;
            }
        }

        return true;
    }

    public override void PostWorldGen() {
        for (int i = 0; i < Main.maxChests; i++) {
            if (Main.chest[i] == null) {
                continue;
            }

            Chest chest = Main.chest[i];

            AddLoot(i);

            UnopenedChestItem.Place(chest);
        }
    }

    private static void AddLoot(int chestId) {
        Chest chest = Main.chest[chestId];
        if (!CanAddLoot(chest)) {
            return;
        }

        Tile tile = Main.tile[chest.x, chest.y];
        ushort wallId = tile.WallType;
        TileObjectData tileObjectData = TileObjectData.GetTileData(tile);
        ushort type = tile.TileType;
        int style = TileHelper.GetStyle(tile, tileObjectData, coordinateFullWidthBackup: 36);

        ChestLootInfo info = new ChestLootInfo(chestId, WorldGen.genRand);
        // Pyramid
        if (wallId == WallID.SandstoneBrick) {
            ChestLootDatabase.Instance.SolveRules(ChestLoot.Pyramid, info);
            return;
        }

        // Dungeon
        if (Main.wallDungeon[wallId]) {
            ChestLootDatabase.Instance.SolveRules(ChestLoot.Dungeon, info);
            return;
        }

        // UG Desert
        if (WallID.Sets.AllowsUndergroundDesertEnemiesToSpawn[wallId]) {
            ChestLootDatabase.Instance.SolveRules(ChestLoot.UndergroundDesert, info);
            return;
        }

        // Temple
        if (wallId == WallID.LihzahrdBrickUnsafe) {
            ChestLootDatabase.Instance.SolveRules(ChestLoot.Temple, info);
            return;
        }

        // Jungle Shrine / Living Wood Tree chest
        if (ChestStyleID.Ivy.Equals(type, style)) {
            ChestLootDatabase.Instance.SolveRules(ChestLoot.Ivy, info);
            return;
        }

        // Surface / Living Tree Chests
        if (chest.y < Main.worldSurface || wallId == WallID.LivingWoodUnsafe) {

            // Sky Island
            if (wallId == WallID.DiscWall || ChestStyleID.Skyware.Equals(type, style) || ChestStyleID.LockedGold.Equals(type, style)) {
                ChestLootDatabase.Instance.SolveRules(ChestLoot.Sky, info);
                return;
            }

            ChestLootDatabase.Instance.SolveRules(ChestLoot.Surface, info);
        }

        // Underground Chests
        else if (chest.y < Main.UnderworldLayer) {

            if (type == TileID.Containers) {
                switch (style) {
                    // Polluted Ocean
                    case ChestStyleID.Containers.TrashCan:
                        return;

                    case ChestStyleID.Containers.Gold:
                        ChestLootDatabase.Instance.SolveRules(ChestLoot.Gold, info);
                        break;
                    case ChestStyleID.Containers.Frozen:
                        ChestLootDatabase.Instance.SolveRules(ChestLoot.Frozen, info);
                        break;
                }
            }

            ChestLootDatabase.Instance.SolveRules(ChestLoot.AllUnderground, info);
        }

        // Underworld Chests
        else if (ChestStyleID.LockedShadow.Equals(type, style)) {
            ChestLootDatabase.Instance.SolveRules(ChestLoot.Shadow, info);
        }
    }
}
