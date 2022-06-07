using Aequus.Common.ItemDrops;
using Aequus.Items.Weapons.Melee;
using Aequus.Tiles;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Aequus.Common
{
    public class LootPools : GlobalItem
    {
        public struct WeightedPool
        {
            public WeightedRandom<Point> Pool { get; private set; }

            public int Chance;

            public WeightedPool(int chance = 1)
            {
                Pool = new WeightedRandom<Point>();
                Chance = chance;
            }

            public bool RollDropChance(UnifiedRandom rand)
            {
                return Chance > 1 ? rand.NextBool(Chance) : true;
            }

            public void Add(int itemID, int stack = 1, double weight = 1f)
            {
                Pool.Add(new Point(itemID, stack), weight);
            }

            public Point RollItem()
            {
                return Pool.Get();
            }

            public void Clear()
            {
                Pool = null;
            }
        }

        public class Bags : GlobalItem
        {
            public static WeightedPool WoodenCrate_Secondary { get; private set; }
            public static WeightedPool Lockbox_Secondary { get; private set; }

            public override void OpenVanillaBag(string context, Player player, int arg)
            {
                if (context == "lockBox")
                {
                    LockboxLoot(player);
                }
                else if (context == "crate")
                {
                    CrateLoot(player, arg);
                }
            }
            public void LockboxLoot(Player player)
            {
                var source = player.GetSource_OpenItem(ItemID.LockBox);
                if (Lockbox_Secondary.RollDropChance(Main.rand))
                {
                    Drop(source, player, Lockbox_Secondary);
                }
            }
            public void CrateLoot(Player player, int type)
            {
                var source = player.GetSource_OpenItem(type);
                if (type == ItemID.WoodenCrate && WoodenCrate_Secondary.RollDropChance(Main.rand))
                {
                    Drop(source, player, WoodenCrate_Secondary);
                }
            }

            public override void Load()
            {
                WoodenCrate_Secondary = new WeightedPool(3);
                Lockbox_Secondary = new WeightedPool(3);
            }

            public override void Unload()
            {
                WoodenCrate_Secondary.Clear();
                Lockbox_Secondary.Clear();
            }
        }

        public class Chests : ModSystem
        {
            public interface IChestLoot
            {
                void AddIntoChest(Chest chest, ref bool anythingAdded);
            }

            public interface IChancedChestLoot : IChestLoot
            {
                public int Chance { get; set; }
                public bool? GuaranteeAtleastOne { get; set; }

                public static bool RollChance(IChancedChestLoot me, UnifiedRandom random)
                {
                    if (me.GuaranteeAtleastOne == true)
                    {
                        me.GuaranteeAtleastOne = false;
                        return true;
                    }
                    return me.Chance > 1 ? random.NextBool(me.Chance) : true;
                }

                public static void RefreshGuarantees(Dictionary<int, Dictionary<int, List<IChestLoot>>> dict)
                {
                    foreach (var d in dict)
                    {
                        foreach (var d2 in d.Value)
                        {
                            foreach (var l in d2.Value)
                            {
                                if (l is IChancedChestLoot l2 && l2.GuaranteeAtleastOne != null)
                                {
                                    l2.GuaranteeAtleastOne = true;
                                }
                            }
                        }
                    }
                }
            }

            public class FrontChestLoot : IChancedChestLoot
            {
                public ItemDrop item;
                public bool insert;
                public bool dontAddInAnythingElseWasAdded;

                public int Chance { get; set; }
                public bool? GuaranteeAtleastOne { get; set; }

                public FrontChestLoot(ItemDrop item, int chance = 1, bool insert = true, bool? guaranteedOne = true, bool dontAddInAnythingElseWasAdded = true)
                {
                    this.item = item;
                    this.insert = insert;
                    this.dontAddInAnythingElseWasAdded = dontAddInAnythingElseWasAdded;
                    Chance = chance;
                    GuaranteeAtleastOne = guaranteedOne;
                }

                public FrontChestLoot(int item, int chance = 1,  bool insert = true, bool? guaranteedOne = true, bool dontAddInAnythingElseWasAdded = true) 
                    : this(new ItemDrop(item, 1), chance, insert, guaranteedOne, dontAddInAnythingElseWasAdded)
                {
                }

                public void AddIntoChest(Chest chest, ref bool anythingAdded)
                {
                    Aequus.Instance.Logger.Debug("Checking if " + Lang.GetItemName(item.item).Value + " is addable. Guarenteed:" + (GuaranteeAtleastOne == null ? "Null" : GuaranteeAtleastOne.Value.ToString()));
                    if (chest.item[0] != null && !chest.item[0].IsAir && IChancedChestLoot.RollChance(this, WorldGen.genRand))
                    {
                        if (insert)
                        {
                            chest.Insert(item.item, item.RollStack(WorldGen.genRand), 1);
                        }
                        else
                        {
                            chest.item[0].SetDefaults(item.item);
                            chest.item[0].stack = item.RollStack(WorldGen.genRand);
                        }
                    }
                }
            }

            public static Dictionary<int, Dictionary<int, List<IChestLoot>>> Pool { get; private set; }

            public override void Load()
            {
                Pool = new Dictionary<int, Dictionary<int, List<IChestLoot>>>();
            }

            public override void Unload()
            {
                Pool?.Clear();
                Pool = null;
            }

            public static void Add(int loot, int chestStyle, int chestTileID = TileID.Containers)
            {
                Add(new FrontChestLoot(loot), chestStyle, chestTileID);
            }

            public static void Add(IChestLoot loot, int chestStyle, int chestTileID = TileID.Containers)
            {
                if (Pool.ContainsKey(chestTileID))
                {
                    if (Pool[chestTileID].ContainsKey(chestStyle))
                    {
                        Pool[chestTileID][chestStyle].Add(loot);
                        return;
                    }

                    Pool[chestTileID].Add(chestStyle, new List<IChestLoot>() { loot });
                    return;
                }

                Pool.Add(chestTileID, new Dictionary<int, List<IChestLoot>>() { [chestStyle] = new List<IChestLoot>() { loot }, });
            }

            public override void PostWorldGen()
            {
                IChancedChestLoot.RefreshGuarantees(Pool);
                var rockmanChests = new List<int>();

                for (int i = 0; i < Main.maxChests; i++)
                {
                    Chest c = Main.chest[i];
                    if (c != null)
                    {
                        if (Pool.TryGetValue(Main.tile[c.x, c.y].TileType, out var d2))
                        {
                            int chestType = ChestTypes.GetChestStyle(c);
                            if (d2.TryGetValue(chestType, out var loot))
                            {
                                bool anythingAdded = false;
                                foreach (var l in loot)
                                {
                                    l.AddIntoChest(c, ref anythingAdded);
                                }
                            }
                        }
                        if (Main.tile[c.x, c.y].TileType == TileID.Containers)
                        {
                            int style = ChestTypes.GetChestStyle(c);
                            if (style == ChestTypes.Gold || style == ChestTypes.Frozen)
                            {
                                rockmanChests.Add(i);
                            }
                        }
                        else if (Main.tile[c.x, c.y].TileType == TileID.Containers2)
                        {
                            int style = ChestTypes.GetChestStyle(c);
                            if (style == ChestTypes.deadMans || style == ChestTypes.sandstone)
                            {
                                rockmanChests.Add(i);
                            }
                        }
                    }
                }

                if (rockmanChests.Count > 0)
                {
                    var c = Main.chest[rockmanChests[WorldGen.genRand.Next(rockmanChests.Count)]];
                    AequusWorld.Structures.Add("RockManChest", new Point(c.x, c.y));
                    c.Insert(ModContent.ItemType<RockMan>(), WorldGen.genRand.Next(Chest.maxItems - 1));
                }
            }
        }

        public static void Drop(IEntitySource source, Player player, WeightedPool drops)
        {
            if (drops.Pool.elements.Count == 0)
            {
                return;
            }
            var item = drops.RollItem();
            player.QuickSpawnItem(source, item.X, item.Y);
        }
    }
}