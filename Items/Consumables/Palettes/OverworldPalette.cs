using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace Aequus.Items.Consumables.Palettes
{
    public class OverworldPalette : PaletteBase
    {
        public static List<int> OverworldChestLoot { get; private set; }

        protected override List<int> LootTable => OverworldChestLoot;

        public override void Load()
        {
            OverworldChestLoot = new List<int>()
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
            OverworldChestLoot?.Clear();
            OverworldChestLoot = null;
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