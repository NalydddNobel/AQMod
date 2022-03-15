using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace AQMod.Items.Misc.ExporterRewards
{
    public class CavernPalette : SchrodingerCrate
    {
        protected override List<int> LootTable => AQItem.Sets.CavernPaletteList;

        private int PoolPotion(int current)
        {
            var choices = AQItem.Sets.CavePotions;
            while (true)
            {
                int choice = Main.rand.Next(choices.Count);
                if (current == -1 || current != choices[choice])
                {
                    return choices[choice];
                }
            }
        }
        public override void RightClick(Player player)
        {
            player.QuickSpawnItem(ItemID.SilverCoin, Main.rand.Next(50, 80));
            if (Main.rand.NextBool())
            {
                player.QuickSpawnItem(ItemID.Extractinator);
            }
            base.RightClick(player);
            if (GetItem() == ItemID.FlareGun)
            {
                player.QuickSpawnItem(ItemID.Flare, Main.rand.Next(25) + 25);
            }

            if (Main.rand.NextBool())
            {
                int p = PoolPotion(-1);
                player.QuickSpawnItem(p, Main.rand.Next(2) + 1);
                if (Main.rand.NextBool())
                {
                    player.QuickSpawnItem(PoolPotion(p), Main.rand.Next(2) + 1);
                }
            }

            if (AQMod.split.IsActive && Main.rand.NextBool())
            {
                player.QuickSpawnItem(AQMod.split.ItemType("ArmorPolishKit"));
            }
        }
    }
}