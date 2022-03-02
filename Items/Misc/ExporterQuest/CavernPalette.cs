using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace AQMod.Items.Misc.ExporterQuest
{
    public class CavernPalette : SchrodingerCrate
    {
        public override string Texture => "Terraria/Item_" + ItemID.IronCrate;

        protected override List<int> LootTable => new List<int>()
        {
            ItemID.BandofRegeneration,
            ItemID.MagicMirror,
            ItemID.CloudinaBottle,
            ItemID.HermesBoots,
            ItemID.EnchantedBoomerang, // Removed in 1.4
            ItemID.ShoeSpikes,
            ItemID.FlareGun,
            //ItemID.Mace, Add in 1.4
        };

        private int PoolPotions(int choice, out int chosen)
        {
            while (true)
            {
                int i = Main.rand.Next(9);
                if (i == choice)
                {
                    continue;
                }
                chosen = i;
                switch (i)
                {
                    case 1:
                        return ItemID.ShinePotion;
                    case 2:
                        return ItemID.NightOwlPotion;
                    case 3:
                        return ItemID.SwiftnessPotion;
                    case 4:
                        return ItemID.ArcheryPotion;
                    case 5:
                        return ItemID.GillsPotion;
                    case 6:
                        return ItemID.HunterPotion;
                    case 7:
                        return ItemID.MiningPotion;
                    case 8:
                        return ItemID.TrapsightPotion;
                }
                return ItemID.RegenerationPotion;
            }
        }
        public override void RightClick(Player player)
        {
            player.QuickSpawnItem(ItemID.SilverCoin, Main.rand.Next(50, 80));
            if (Main.rand.NextBool())
            {
                player.QuickSpawnItem(PoolPotions(-1, out int choice), Main.rand.Next(2) + 1);
                if (Main.rand.NextBool())
                {
                    player.QuickSpawnItem(PoolPotions(choice, out _), Main.rand.Next(2) + 1);
                }
            }
            if (Main.rand.NextBool())
            {
                player.QuickSpawnItem(ItemID.Extractinator);
            }
            base.RightClick(player);
            if (GetItem() == ItemID.FlareGun)
            {
                player.QuickSpawnItem(ItemID.Flare, Main.rand.Next(25) + 25);
            }
        }
    }
}