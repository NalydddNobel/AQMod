using Aequus.Common.ID;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.WorldGeneration
{
    public sealed class CustomChestLoot : ModSystem
    {
        public struct ChestLootData
        {
            public int originalItem;
            public int newItem;
            public int replacementChance;

            public ChestLootData(int original, int newItem) : this(original, newItem, 2)
            {
            }

            public ChestLootData(int original, int newItem, int chance)
            {
                originalItem = original;
                this.newItem = newItem;
                replacementChance = chance;
            }

            public bool RollChance()
            {
                return replacementChance <= 1 || WorldGen.genRand.NextBool(replacementChance);
            }
        }

        public static Dictionary<int, List<ChestLootData>> VanillaChestRewards { get; private set; }

        public override void Load()
        {
            VanillaChestRewards = new Dictionary<int, List<ChestLootData>>();
            for (int i = 0; i < ChestTypes.VanillaCount; i++)
            {
                VanillaChestRewards[i] = new List<ChestLootData>();
            }
        }

        public override void PostWorldGen()
        {
            for (int i = 0; i < Main.maxChests; i++)
            {
                Chest c = Main.chest[i];
                if (c != null && Main.tile[c.x, c.y].TileType == TileID.Containers && c.item[0] != null && !c.item[0].IsAir)
                {
                    if (VanillaChestRewards.TryGetValue(ChestTypes.GetChestType(c), out var list))
                    {
                        foreach (var loot in list)
                        {
                            if (c.item[0].type == loot.originalItem)
                            {
                                if (loot.RollChance())
                                {
                                    c.item[0].SetDefaults(loot.newItem);
                                }
                                continue;
                            }
                        }
                    }
                }
            }
        }

        public override void Unload()
        {
            VanillaChestRewards?.Clear();
            VanillaChestRewards = null;
        }
    }
}