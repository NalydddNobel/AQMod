using AequusRemake.Content.Chests;
using AequusRemake.DataSets;
using AequusRemake.Systems.Chests.DropRules;
using Terraria.ObjectData;

namespace AequusRemake.Systems.Chests;

public sealed class ChestLootSystem : ModSystem {
    private static bool CanAddLoot(Chest chest) {
        for (int m = 0; m < Chest.maxItems; m++) {
            if (chest.item[m] == null || chest.item[m].IsAir) {
                continue;
            }

            // Prevent AequusRemake from adjusting chest loot at all if it contains an important item
            if (ItemDataSet.ImportantItem.Contains(chest.item[m].type)) {
                return false;
            }
        }

        return true;
    }

    public override void ClearWorld() {
        ChestLootDatabase.Instance.OnClearWorld();
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

        ChestStyle style = ChestStyleConversion.ToEnum(tile);

        ChestLootInfo info = new ChestLootInfo(chestId, WorldGen.genRand);
        // Pyramid
        if (wallId == WallID.SandstoneBrick) {
            ChestLootDatabase.Instance.SolveRules(ChestPool.Pyramid, in info);
            return;
        }

        // Dungeon
        if (Main.wallDungeon[wallId] && style == ChestStyle.LockedGold) {
            ChestLootDatabase.Instance.SolveRules(ChestPool.Dungeon, in info);
            return;
        }

        // UG Desert
        if (WallID.Sets.AllowsUndergroundDesertEnemiesToSpawn[wallId]) {
            ChestLootDatabase.Instance.SolveRules(ChestPool.UndergroundDesert, in info);
            return;
        }

        // Temple
        if (wallId == WallID.LihzahrdBrickUnsafe) {
            ChestLootDatabase.Instance.SolveRules(ChestPool.Temple, in info);
            return;
        }

        // Jungle Shrine / Living Wood Tree chest
        if (style == ChestStyle.Ivy) {
            ChestLootDatabase.Instance.SolveRules(ChestPool.Ivy, in info);
            return;
        }

        // Surface / Living Tree Chests
        if (chest.y < Main.worldSurface || wallId == WallID.LivingWoodUnsafe) {

            // Sky Island
            if (wallId == WallID.DiscWall || style == ChestStyle.Skyware || style == ChestStyle.LockedGold) {
                ChestLootDatabase.Instance.SolveRules(ChestPool.Sky, in info);
                return;
            }

            ChestLootDatabase.Instance.SolveRules(ChestPool.Surface, in info);
        }

        // Underground Chests
        else if (chest.y < Main.UnderworldLayer) {

            switch (style) {
                // Polluted Ocean
                case ChestStyle.TrashCan:
                    return;

                case ChestStyle.Gold:
                    ChestLootDatabase.Instance.SolveRules(ChestPool.Gold, in info);
                    break;
                case ChestStyle.Frozen:
                    ChestLootDatabase.Instance.SolveRules(ChestPool.Frozen, in info);
                    break;
            }

            ChestLootDatabase.Instance.SolveRules(ChestPool.Underground, in info);
        }

        // Underworld Chests
        else if (style == ChestStyle.LockedShadow) {
            ChestLootDatabase.Instance.SolveRules(ChestPool.Shadow, in info);
        }
    }
}
