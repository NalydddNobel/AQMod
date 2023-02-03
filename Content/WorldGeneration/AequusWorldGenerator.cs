using Aequus.Items.Accessories.Debuff;
using Aequus.Items.Accessories.Summon.Necro;
using Aequus.Items.Accessories.Utility;
using Aequus.Items.Accessories.Vanity.Cursors;
using Aequus.Items.Pets;
using Aequus.Items.Pets.Light;
using Aequus.Items.Tools;
using Aequus.Items.Weapons.Melee;
using Aequus.Items.Weapons.Ranged;
using Aequus.Items.Weapons.Summon.Candles;
using Aequus.Items.Weapons.Summon.Scepters;
using Aequus.Tiles;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;
using Terraria.WorldBuilding;

namespace Aequus.Content.WorldGeneration
{
    public class AequusWorldGenerator : ModSystem
    {
        public static CrabCreviceGenerator GenCrabCrevice { get; private set; }
        public static GoreNestGenerator GenGoreNest { get; private set; }
        public static RockmanChestGenerator RockmanGenerator { get; private set; }
        public static RadonCaveGenerator RadonCaves { get; private set; }

        public override void Load()
        {
            RadonCaves = new RadonCaveGenerator();
            RockmanGenerator = new RockmanChestGenerator();
            GenCrabCrevice = new CrabCreviceGenerator();
            GenGoreNest = new GoreNestGenerator();
        }

        public override void Unload()
        {
            RockmanGenerator = null;
            GenCrabCrevice = null;
            GenGoreNest = null;
        }

        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
        {
            AequusWorld.Structures = new StructureLookups();
            AddPass("Shinies", "Radon Biome", (progress, configuration) =>
            {
                progress.Message = Language.GetTextValue("Mods.Aequus.WorldGeneration.RadonBiome");
                RadonCaves.GenerateWorld();
            }, tasks);
            AddPass("Statues", "Rockman Biome", (progress, configuration) =>
            {
                RockmanGenerator.GenerateRandomLocation();
            }, tasks);
            AddPass("Beaches", "Crab Home", (progress, configuration) =>
            {
                progress.Message = AequusText.GetText("WorldGeneration.CrabCrevice");
                GenCrabCrevice.Generate(progress);
            }, tasks);
            AddPass("Underworld", "Gore Nests", (progress, configuration) =>
            {
                progress.Message = AequusText.GetText("WorldGeneration.GoreNests");
                GenGoreNest.Generate();
            }, tasks);
            AddPass("Pots", "Crab Pottery", (progress, configuration) =>
            {
                progress.Message = AequusText.GetText("WorldGeneration.CrabCrevicePots");
                GenCrabCrevice.TransformPots();
            }, tasks);
            AddPass("Tile Cleanup", "Gore Nest Cleanup", (progress, configuration) =>
            {
                progress.Message = AequusText.GetText("WorldGeneration.GoreNestCleanup");
                GenGoreNest.Cleanup();
            }, tasks);
            AddPass("Tile Cleanup", "Crab Growth", (progress, configuration) =>
            {
                progress.Message = AequusText.GetText("WorldGeneration.CrabCreviceGrowth");
                GenCrabCrevice.GrowPlants();
            }, tasks);
        }
        public static void AddPass(string task, string myName, WorldGenLegacyMethod generation, List<GenPass> tasks)
        {
            int i = tasks.FindIndex((t) => t.Name.Equals(task));
            if (i != -1)
                tasks.Insert(i + 1, new PassLegacy("Aequus: " + myName, generation));
        }
        public static void CopyPass(string name, int amt, List<GenPass> tasks)
        {
            int i = tasks.FindIndex((t) => t.Name.Equals(name));
            if (i != -1)
            {
                var t = tasks[i];
                for (int k = 0; k < amt; k++)
                {
                    tasks.Insert(i, new PassLegacy($"Aequus: {t.Name} {i}", (p, c) => t.Apply(p, c)));
                }
            }
        }
        public static void RemovePass(string name, List<GenPass> tasks)
        {
            int i = tasks.FindIndex((t) => t.Name.Equals(name));
            if (i != -1)
            {
                tasks[i] = new PassLegacy(tasks[i].Name, (p, c) => { });
            }
        }

        public override void PostWorldGen()
        {
            var rockmanChests = new List<int>();

            var placedItems = new HashSet<int>();
            var r = WorldGen.genRand;
            for (int k = 0; k < Main.maxChests; k++)
            {
                Chest c = Main.chest[k];
                if (c != null)
                {
                    int style = ChestType.GetStyle(c);
                    if (Main.tile[c.x, c.y].TileType == TileID.Containers)
                    {
                        if (style == ChestType.Gold || style == ChestType.Marble || style == ChestType.Granite || style == ChestType.Mushroom || style == ChestType.RichMahogany)
                        {
                            UndergroundChestLoot(k, c, rockmanChests, placedItems, r);
                        }
                        else if (style == ChestType.LockedGold && Main.wallDungeon[Main.tile[c.x, c.y].WallType])
                        {
                            int choice = -1;
                            for (int i = 0; i < 4; i++)
                            {
                                int item = DungeonChestItem(i);
                                if (!placedItems.Contains(item))
                                {
                                    choice = item;
                                }
                            }
                            if (choice == -1 && r.NextBool(4))
                            {
                                choice = DungeonChestItem(r.Next(4));
                            }

                            if (choice != -1)
                            {
                                c.Insert(choice, 1);
                                placedItems.Add(choice);
                            }
                        }
                        else if (style == ChestType.Frozen)
                        {
                            rockmanChests.Add(k);

                            if (r.NextBool(6))
                            {
                                AddGlowCore(c, placedItems);
                            }
                            if (!placedItems.Contains(ModContent.ItemType<CrystalDagger>()) || r.NextBool(6))
                            {
                                c.Insert(ModContent.ItemType<CrystalDagger>(), 1);
                                placedItems.Add(ModContent.ItemType<CrystalDagger>());
                            }
                        }
                        else if (style == ChestType.Skyware || (style == ChestType.LockedGold && !Main.wallDungeon[Main.tile[c.x, c.y].WallType] && c.y < (int)Main.worldSurface))
                        {
                            if (!placedItems.Contains(ModContent.ItemType<Slingshot>()) || r.NextBool())
                            {
                                c.Insert(ModContent.ItemType<Slingshot>(), 1);
                                placedItems.Add(ModContent.ItemType<Slingshot>());
                            }
                        }
                    }
                    else if (Main.tile[c.x, c.y].TileType == TileID.Containers2)
                    {
                        if (style == ChestType.DeadMans)
                        {
                            UndergroundChestLoot(k, c, rockmanChests, placedItems, r);
                        }
                        else if (style == ChestType.Sandstone)
                        {
                            rockmanChests.Add(k);
                        }
                    }
                    if (WorldGen.genRand.NextBool(7))
                    {
                        for (int i = 0; i < Chest.maxItems; i++)
                        {
                            if (!c.item[i].IsAir && c.item[i].type == ItemID.SuspiciousLookingEye)
                            {
                                c.item[i].SetDefaults<SwagLookingEye>();
                                break;
                            }
                        }
                    }
                }
            }
        }
        public static void UndergroundChestLoot(int k, Chest c, List<int> rockmanChests, HashSet<int> placedItems, UnifiedRandom r)
        {
            rockmanChests.Add(k);

            if (!placedItems.Contains(ModContent.ItemType<SwordCursor>()) || r.NextBool(20))
            {
                for (int i = 0; i < Chest.maxItems; i++)
                {
                    if (c.item[i].IsAir)
                    {
                        c.item[i].SetDefaults(ModContent.ItemType<SwordCursor>());
                        placedItems?.Add(ModContent.ItemType<SwordCursor>());
                        break;
                    }
                }
            }
            if (r.NextBool(5))
            {
                AddGlowCore(c, placedItems);
            }

            switch (r.Next(5))
            {
                case 0:
                    c.Insert(ModContent.ItemType<BoneRing>(), 1);
                    break;

                case 1:
                    c.Insert(ModContent.ItemType<BattleAxe>(), 1);
                    break;

                case 2:
                    c.Insert(ModContent.ItemType<Bellows>(), 1);
                    break;
            }
        }
        public static bool AddGlowCore(Chest c, HashSet<int> placedItems = null)
        {
            for (int i = 0; i < Chest.maxItems; i++)
            {
                if (!c.item[i].IsAir && (c.item[i].type == ItemID.Torch || c.item[i].type == ItemID.Glowstick))
                {
                    if ((WorldGen.genRand.NextBool() && placedItems.Contains(ModContent.ItemType<MiningPetSpawner>())) || !placedItems.Contains(ModContent.ItemType<GlowCore>()))
                    {
                        c.item[i].SetDefaults(ModContent.ItemType<GlowCore>());
                        placedItems?.Add(ModContent.ItemType<GlowCore>());
                    }
                    else
                    {
                        c.item[i].SetDefaults(ModContent.ItemType<MiningPetSpawner>());
                        placedItems?.Add(ModContent.ItemType<MiningPetSpawner>());
                    }
                    return true;
                }
            }
            return false;
        }
        public static int DungeonChestItem(int type)
        {
            switch (WorldGen.genRand.Next(4))
            {
                default:
                    return ModContent.ItemType<Valari>();
                case 1:
                    return ModContent.ItemType<Revenant>();
                case 2:
                    return ModContent.ItemType<DungeonCandle>();
                case 3:
                    return ModContent.ItemType<PandorasBox>();
            }
        }
        
        public static bool CanPlaceStructure(int middleX, int middleY, int width, int height, int padding = 0)
        {
            return CanPlaceStructure(new Rectangle(middleX - width / 2, middleY - height / 2, width, height), padding);
        }
        public static bool CanPlaceStructure(Rectangle rect, int padding = 0)
        {
            return WorldGen.structures?.CanPlace(rect) != false;
        }

        public static bool CanPlaceStructure(int middleX, int middleY, int width, int height, bool[] invalidTiles, int padding = 0)
        {
            return CanPlaceStructure(new Rectangle(middleX - width / 2, middleY - height / 2, width, height), padding);
        }
        public static bool CanPlaceStructure(Rectangle rect, bool[] invalidTiles, int padding = 0)
        {
            return WorldGen.structures?.CanPlace(rect, invalidTiles, padding) != false;
        }
    }
}