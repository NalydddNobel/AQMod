using Aequus.Common.Tiles;
using Aequus.Content.Configuration;
using Aequus.Content.DataSets;
using Aequus.Content.Tools.NameTag;
using Aequus.Content.VanillaChanges;
using Terraria.ObjectData;

namespace Aequus.Content.WorldGeneration;
public class BuriedChestLoot : ModSystem {
    public static int SlimeCrownSpawnrate { get; set; } = 7;

    public static void CheckSurfaceChest(Chest chest) {
        if (VanillaChangesConfig.Instance.SlimeCrownInSurfaceChests && WorldGen.genRand.NextBool(SlimeCrownSpawnrate) && !chest.item.Any(i => i.type == ItemID.SlimeCrown)) {
            chest.AddItem(ItemID.SlimeCrown);
        }
    }

    public static void CheckUGGoldChest(Chest chest) {
        if (VanillaChangesConfig.Instance.MoveToolbelt && WorldGen.genRand.NextBool(ToolbeltChanges.SpawnRate)) {
            chest.AddItem(ItemID.Toolbelt);
        }
        if (WorldGen.genRand.NextBool(NameTag.ChestSpawnrate)) {
            chest.AddItem(ModContent.ItemType<NameTag>());
        }
    }

    public static void CheckShadowChest(Chest chest) {
        if (VanillaChangesConfig.Instance.MoveTreasureMagnet) {
            chest.ReplaceFirst(ItemID.TreasureMagnet, ItemID.HellstoneBar, WorldGen.genRand.Next(10, 21));
        }
    }

    public override void PostWorldGen() {
        for (int k = 0; k < Main.maxChests; k++) {
            Chest chest = Main.chest[k];
            if (chest == null || !WorldGen.InWorld(chest.x, chest.y, 40) || !CanAddLoot(chest)) {
                continue;
            }

            Tile tile = Main.tile[chest.x, chest.y];
            ushort wallId = tile.WallType;
            TileObjectData tileObjectData = TileObjectData.GetTileData(tile);
            ushort type = tile.TileType;
            int style = TileHelper.GetStyle(tile, tileObjectData, coordinateFullWidthBackup: 36);

            // Pyramid
            if (wallId == WallID.SandstoneBrick) {
                continue;
            }

            // Dungeon
            if (Main.wallDungeon[wallId]) {
                continue;
            }

            // UG Desert
            if (WallID.Sets.AllowsUndergroundDesertEnemiesToSpawn[wallId]) {
                continue;
            }

            // Temple
            if (wallId == WallID.LihzahrdBrickUnsafe) {
                continue;
            }

            // Jungle Shrine / Living Wood Tree chest
            if (type == TileID.Containers && style == ChestType.Ivy) {
                continue;
            }

            // Surface / Living Tree Chests
            if (chest.y < Main.worldSurface || wallId == WallID.LivingWoodUnsafe) {

                // Sky Island
                if (wallId == WallID.DiscWall || (type == TileID.Containers && style == ChestType.Skyware)) {
                    continue;
                }

                CheckSurfaceChest(chest);
            }

            // Underground Chests
            else if (chest.y < Main.UnderworldLayer) {
                CheckUGGoldChest(chest);
            }

            // Underworld Chests
            else if (type == TileID.Containers && style == ChestType.LockedShadow) {
                CheckShadowChest(chest);
            }
        }
    }

    private static bool CanAddLoot(Chest chest) {
        for (int m = 0; m < Chest.maxItems; m++) {
            if (chest.item[m] == null || chest.item[m].IsAir) {
                continue;
            }

            // Prevent Aequus from adjusting chest loot at all if it contains an important item
            if (ItemSets.ImportantItem.Contains(chest.item[m].type)) {
                return false;
            }
        }

        return true;
    }
}