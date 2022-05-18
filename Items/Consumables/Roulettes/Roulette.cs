using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace Aequus.Items.Consumables.Roulettes
{
    public class Roulette : RouletteBase
    {
        public static List<int> Table { get; private set; }

        protected override List<int> LootTable => Table;

        public override void Load()
        {
            Table = new List<int>()
                {
                    ItemID.Spear,
                    ItemID.Blowpipe,
                    ItemID.WoodenBoomerang,
                    ItemID.Aglet,
                    ItemID.ClimbingClaws,
                    ItemID.Umbrella,
                    ItemID.WandofSparking,
                    ItemID.Radar,
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
            player.QuickSpawnItem(source, ItemID.SilverCoin, Main.rand.Next(10, 40));
            if (Main.rand.NextBool())
            {
                player.QuickSpawnItem(source, ItemID.HerbBag);
            }
            if (Main.rand.NextBool())
            {
                player.QuickSpawnItem(source, ItemID.CanOfWorms);
            }
            base.RightClick(player);
        }
    }
}