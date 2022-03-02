using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace AQMod.Items.Misc.ExporterQuest
{
    public class OverworldPalette : SchrodingerCrate
    {
        public override string Texture => "Terraria/Item_" + ItemID.WoodenCrate;

        protected override List<int> LootTable => new List<int>()
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