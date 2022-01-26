using AQMod.Common.Configuration;
using AQMod.Common.ID;
using AQMod.Items.Tools.Map;
using AQMod.Items.Weapons.Melee;
using AQMod.Tiles.Furniture.Containers;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.World.Generation;

namespace AQMod.Content.World.Generation
{
    public class CustomChestLoot : ModWorld
    {
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
        {
            int i = tasks.FindIndex((t) => t.Name.Equals("Micro Biomes"));
            if (i != -1)
            {
                tasks.Insert(i + 1, new PassLegacy("AQMod: Buried Chests", GenerateDirtChests));
            }
        }

        public override void PostWorldGen()
        {
            for (int i = 0; i < Main.maxChests; i++)
            {
                Chest c = Main.chest[i];
                if (c != null && Main.tile[c.x, c.y].type == TileID.Containers)
                {
                    AddLoot(i);
                }
            }
        }

        public static bool CanPlaceBuriedChest(int i, int j)
        {
            for (int x = i - 7; x < i + 9; x++)
            {
                for (int y = j - 8; y < j + 8; y++)
                {
                    if (!Main.tile[x, y].active() || !Main.tileSolid[Main.tile[x, y].type] || !Main.tileBlockLight[Main.tile[x, y].type])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static bool PlaceBuriedChest(int i, int j, out int chestID, int validTileType, int placeWallType = WallID.Dirt, int style = 0)
        {
            chestID = -1;
            if (Main.tile[i, j] == null)
            {
                Main.tile[i, j] = new Tile();
                return false;
            }
            if (Main.tile[i, j].type != validTileType)
            {
                return false;
            }
            if (CanPlaceBuriedChest(i, j))
            {
                Main.tile[i, j].active(active: false);
                Main.tile[i + 1, j].active(active: false);
                Main.tile[i, j - 1].active(active: false);
                Main.tile[i + 1, j - 1].active(active: false);

                Main.tile[i, j].wall = (ushort)placeWallType;
                Main.tile[i + 1, j].wall = (ushort)placeWallType;
                Main.tile[i, j - 1].wall = (ushort)placeWallType;
                Main.tile[i + 1, j - 1].wall = (ushort)placeWallType;

                WorldGen.PlaceTile(i, j, ModContent.TileType<BuriedChests>(), mute: true, forced: true, style: style);
                if (Main.tile[i, j].type == ModContent.TileType<BuriedChests>() && (chestID = Chest.FindChest(i, j - 1)) != -1)
                {
                    return true;
                }
            }
            return false;
        }

        public static void GenerateDirtChests(GenerationProgress progress)
        {
            if (progress != null)
                progress.Message = Language.GetTextValue("Mods.AQMod.WorldGen.BuriedDirtChests");

            int chestLootID = 0;
            for (int i = 0; i < Main.maxTilesX; i++)
            {
                int x = WorldGen.genRand.Next(100, Main.maxTilesX - 100);
                int y = WorldGen.genRand.Next(300, 750);
                PlaceBuriedChest(x, y, out int chestID, TileID.Dirt, WallID.Dirt, 0);
                if (chestID != -1)
                {
                    i += 100;
                    if (Main.chest[chestID] == null)
                        Main.chest[chestID] = new Chest();
                    var c = Main.chest[chestID];
                    switch (chestLootID)
                    {
                        case 0:
                            c.item[0].SetDefaults(ItemID.HermesBoots);
                            break;
                        case 1:
                            c.item[0].SetDefaults(ItemID.CloudinaBottle);
                            break;
                        case 2:
                            c.item[0].SetDefaults(ItemID.Radar);
                            break;
                    }
                    int currentIndex = 1;
                    if (WorldGen.genRand.NextBool())
                    {
                        c.item[currentIndex].SetDefaults(ItemID.MagicMirror);
                        currentIndex++;
                    }
                    if (WorldGen.genRand.NextBool())
                    {
                        c.item[currentIndex].SetDefaults(ItemID.HerbBag);
                        currentIndex++;
                    }
                    if (WorldGen.genRand.NextBool())
                    {
                        c.item[currentIndex].SetDefaults(ItemID.Grenade);
                        c.item[currentIndex].stack = 3 + WorldGen.genRand.Next(3);
                        currentIndex++;
                    }
                    if (WorldGen.genRand.NextBool())
                    {
                        c.item[currentIndex].SetDefaults(WorldGen.copperBar);
                        c.item[currentIndex].stack = 3 + WorldGen.genRand.Next(8);
                    }
                    else
                    {
                        c.item[currentIndex].SetDefaults(WorldGen.ironBar);
                        c.item[currentIndex].stack = 3 + WorldGen.genRand.Next(8);
                    }
                    currentIndex++;
                    if (WorldGen.genRand.NextBool())
                    {
                        c.item[currentIndex].SetDefaults(ItemID.Rope);
                        c.item[currentIndex].stack = 50 + WorldGen.genRand.Next(51);
                        currentIndex++;
                    }
                    if (WorldGen.genRand.NextBool())
                    {
                        c.item[currentIndex].SetDefaults(ItemID.HealingPotion);
                        c.item[currentIndex].stack = 3 + WorldGen.genRand.Next(4);
                        currentIndex++;
                    }
                    c.item[currentIndex].SetDefaults(ItemID.Torch);
                    c.item[currentIndex].stack = 12 + WorldGen.genRand.Next(17);
                    currentIndex++;
                    if (WorldGen.genRand.NextBool())
                    {
                        c.item[currentIndex].SetDefaults(ItemID.Bottle);
                        c.item[currentIndex].stack = 12 + WorldGen.genRand.Next(17);
                        currentIndex++;
                    }
                    c.item[currentIndex].SetDefaults(ItemID.Wood);
                    c.item[currentIndex].stack = 50 + WorldGen.genRand.Next(251);
                    currentIndex++;
                    chestLootID = (chestLootID + 1) % 3;
                }
            }
        }

        public static void AddLoot(int i)
        {
            Chest c = Main.chest[i];
            switch (ChestStyles.GetChestStyle(c))
            {
                case ChestStyles.Wood:
                    {
                        if (WorldGen.genRand.NextBool(4))
                        {
                            MainLoot(c, ModContent.ItemType<Items.Weapons.Melee.VineSword>());
                        }
                    }
                    break;

                case ChestStyles.Ice:
                    {
                        if (WorldGen.genRand.NextBool(4))
                        {
                            MainLoot(c, ModContent.ItemType<CrystalDagger>());
                        }
                    }
                    break;

                case ChestStyles.LockedGold:
                    {
                        if (Main.wallDungeon[Main.tile[c.x, c.y].type] && WorldGen.genRand.NextBool(3))
                        {
                            InsertLoot(c, ModContent.ItemType<DungeonMap>(), 1, CountAllActiveItemIndices(c));
                        }
                    }
                    break;

                case ChestStyles.Lihzahrd:
                    {
                        if (WorldGen.genRand.NextBool(3))
                        {
                            InsertLoot(c, ModContent.ItemType<LihzahrdMap>(), 1, CountAllActiveItemIndices(c));
                        }
                    }
                    break;
            }
        }

        public static void MainLoot(Chest chest, int item)
        {
            if (ModContent.GetInstance<WorldGenOptions>().overrideVanillaChestLoot)
            {
                chest.item[0].SetDefaults(item);
            }
            else
            {
                InsertLoot(chest, item, 0, CountAllActiveItemIndices(chest));
            }
        }

        public static int CountAllActiveItemIndices(Chest chest)
        {
            int i = 0;
            for (int j = 0; j < Chest.maxItems; j++)
            {
                if (chest.item[j] != null && chest.item[j].type > ItemID.None)
                {
                    i++;
                }
            }
            return i;
        }

        public static void InsertLoot(Chest shop, int itemID, int interceptPoint, int maxItems)
        {
            if (maxItems >= Chest.maxItems)
                maxItems = Chest.maxItems - 1;
            for (int j = maxItems; j > interceptPoint; j--)
            {
                shop.item[j] = shop.item[j - 1];
            }
            shop.item[interceptPoint] = new Item(); // removes the object reference in the slot after this
            shop.item[interceptPoint].SetDefaults(itemID);
        }
    }
}