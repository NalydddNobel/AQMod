using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace AQMod.Items.Misc.ExporterRewards
{
    public class SkyPalette : SchrodingerCrate
    {
        public override string Texture => "Terraria/Item_" + ItemID.FloatingIslandFishingCrate;

        protected override List<int> LootTable => AQItem.Sets.Instance.SkyChestLoot;

        public override void RightClick(Player player)
        {
            player.QuickSpawnItem(ItemID.SilverCoin, Main.rand.Next(50, 80));

            if (Main.rand.NextBool())
            {
                player.QuickSpawnItem(ItemID.SkyMill);
            }

            base.RightClick(player);

            if (Main.rand.NextBool())
            {
                int p = AQItem.PoolPotion(-1);
                player.QuickSpawnItem(p, Main.rand.Next(2) + 1);
                if (Main.rand.NextBool())
                {
                    player.QuickSpawnItem(AQItem.PoolPotion(p), Main.rand.Next(2) + 1);
                }
            }
        }
    }
}