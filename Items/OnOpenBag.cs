using Aequus.Common.ItemDrops;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items
{
    public class OnOpenBag : GlobalItem
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
                OpenBag_Lockbox(player);
            }
            else if (context == "crate")
            {
                OpenBag_Crate(player, arg);
            }
        }

        public void OpenBag_Lockbox(Player player)
        {
            var source = player.GetSource_OpenItem(ItemID.LockBox);
            if (Main.rand.NextBool(3))
            {
                DropHelper.OneFromList(source, player, LockboxPool);
            }
        }

        public void OpenBag_Crate(Player player, int type)
        {
            var source = player.GetSource_OpenItem(type);
            if (type == ItemID.WoodenCrate && Main.rand.NextBool(3))
            {
                DropHelper.OneFromList(source, player, WoodenCratePool);
            }
        }
    }
}