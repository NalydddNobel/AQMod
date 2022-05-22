using Aequus.Common.ItemDrops;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items
{
    public class BagLootPools : GlobalItem
    {
        public static List<int> WoodenCratePool { get; private set; }
        public static List<int> LockboxPool { get; private set; }

        public override void Load()
        {
            WoodenCratePool = new List<int>();
            LockboxPool = new List<int>();
        }

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
            if (Main.rand.NextBool(3))
            {
                DropHelper.OneFromList(source, player, LockboxPool);
            }
        }
        public void CrateLoot(Player player, int type)
        {
            var source = player.GetSource_OpenItem(type);
            if (type == ItemID.WoodenCrate && Main.rand.NextBool(3))
            {
                DropHelper.OneFromList(source, player, WoodenCratePool);
            }
        }
    }
}