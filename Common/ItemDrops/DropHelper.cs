using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;

namespace Aequus.Common.ItemDrops
{
    public class DropHelper
    {
        public static void OneFromList(IEntitySource source, Player player, List<int> drops)
        {
            if (drops.Count == 0)
            {
                return;
            }
            player.QuickSpawnItem(source, drops[Main.rand.Next(drops.Count)], 1);
        }
    }
}