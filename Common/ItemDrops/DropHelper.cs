using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;

namespace Aequus.Common.ItemDrops
{
    public sealed class DropHelper
    {
        private static int ChooseFromList(List<int> list)
        {
            return list.Count > 1 ? list[Main.rand.Next(list.Count)] : list[0];
        }

        public static void OneFromList(IEntitySource source, Player player, List<int> drops)
        {
            if (drops.Count == 0)
            {
                return;
            }
            player.QuickSpawnItem(source, ChooseFromList(drops), 1);
        }
    }
}