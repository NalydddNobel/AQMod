using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace Aequus.Items.Consumables.Roulettes
{
    public class SkyRoulette : RouletteBase
    {
        public static List<int> Table { get; private set; }

        protected override List<int> LootTable => Table;

        public override void Load()
        {
            Table = new List<int>()
                {
                    ItemID.ShinyRedBalloon,
                    ItemID.CreativeWings,
                    ItemID.Starfury,
                    //ModContent.ItemType<DreamCatcher>(),
                };
        }

        public override void Unload()
        {
            Table?.Clear();
            Table = null;
        }

        public override void RightClick(Player player)
        {
            var source = player.GetSource_OpenItem(Type);
            player.QuickSpawnItem(source, ItemID.SilverCoin, Main.rand.Next(50, 80));

            if (Main.rand.NextBool())
            {
                player.QuickSpawnItem(source, ItemID.SkyMill);
            }

            base.RightClick(player);

            //if (Main.rand.NextBool())
            //{
            //    int p = AQItem.PoolPotion(-1);
            //    player.QuickSpawnItem(p, Main.rand.Next(2) + 1);
            //    if (Main.rand.NextBool())
            //    {
            //        player.QuickSpawnItem(AQItem.PoolPotion(p), Main.rand.Next(2) + 1);
            //    }
            //}
        }
    }
}