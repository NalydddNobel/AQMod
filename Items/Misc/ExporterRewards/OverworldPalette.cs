using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace AQMod.Items.Misc.ExporterRewards
{
    public class OverworldPalette : SchrodingerCrate
    {
        protected override List<int> LootTable => AQItem.Sets.OverworldPaletteList;

        public override void RightClick(Player player)
        {
            player.QuickSpawnItem(ItemID.SilverCoin, Main.rand.Next(10, 40));
            if (Main.rand.NextBool())
            {
                player.QuickSpawnItem(ItemID.HerbBag);
            }
            base.RightClick(player);
        }
    }
}