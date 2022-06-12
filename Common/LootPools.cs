using Microsoft.Xna.Framework;
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