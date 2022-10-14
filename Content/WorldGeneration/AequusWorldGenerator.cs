using Aequus.Items.Accessories;
using Aequus.Items.Accessories.Summon.Necro;
using Aequus.Items.Pets;
using Aequus.Items.Tools;
using Aequus.Items.Weapons.Melee;
using Aequus.Items.Weapons.Ranged;
using Aequus.Items.Weapons.Summon.Necro;
using Aequus.Items.Weapons.Summon.Necro.Candles;
using Aequus.Tiles;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Aequus.Content.WorldGeneration
{
    public class AequusWorldGenerator : ModSystem
    {
        public static CrabCreviceGenerator GenCrabCrevice { get; private set; }
        public static GoreNestGenerator GenGoreNest { get; private set; }

        public override void Load()
        {
            GenCrabCrevice = new CrabCreviceGenerator();
            GenGoreNest = new GoreNestGenerator();
        }

        public override void Unload()
        {
            GenCrabCrevice = null;
        }

        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
        {
            AequusWorld.Structures = new StructureLookups();
            AddPass("Beaches", "Crab Home", (progress, configuration) =>
            {
                progress.Message = AequusText.GetText("WorldGeneration.CrabCrevice");
                GenCrabCrevice.Generate(null);
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
        private void AddPass(string task, string myName, WorldGenLegacyMethod generation, List<GenPass> tasks)
        {
            int i = tasks.FindIndex((t) => t.Name.Equals(task));
            if (i != -1)
                tasks.Insert(i + 1, new PassLegacy("Aequus: " + myName, generation));
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
                    if (Main.tile[c.x, c.y].TileType == TileID.Containers)
                    {
                        int style = ChestTypes.GetChestStyle(c);
                        if (style == ChestTypes.Gold || style == ChestTypes.Marble || style == ChestTypes.Granite || style == ChestTypes.Mushroom)
                        {
                            rockmanChests.Add(k);

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
                        else if (style == ChestTypes.LockedGold)
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
                        else if (style == ChestTypes.Frozen)
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
                        else if (style == ChestTypes.Skyware)
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
                        int style = ChestTypes.GetChestStyle(c);
                        if (style == ChestTypes.deadMans)
                        {
                            rockmanChests.Add(k);
                            if (r.NextBool())
                            {
                                AddGlowCore(c, placedItems);
                            }
                        }
                        else if (style == ChestTypes.sandstone)
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

            if (rockmanChests.Count > 0)
            {
                var c = Main.chest[rockmanChests[r.Next(rockmanChests.Count)]];
                AequusWorld.Structures.Add("RockManChest", new Point(c.x, c.y));
                c.Insert(ModContent.ItemType<RockMan>(), r.Next(Chest.maxItems - 1));
            }
        }
        public static bool AddGlowCore(Chest c, HashSet<int> placedItems = null)
        {
            for (int i = 0; i < Chest.maxItems; i++)
            {
                if (!c.item[i].IsAir && (c.item[i].type == ItemID.Torch || c.item[i].type == ItemID.Glowstick))
                {
                    c.item[i].SetDefaults(ModContent.ItemType<GlowCore>());
                    placedItems?.Add(ModContent.ItemType<GlowCore>());
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
    }
}