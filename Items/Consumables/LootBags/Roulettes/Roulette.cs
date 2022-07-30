using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Consumables.LootBags.Roulettes
{
    public class Roulette : RouletteBase
    {
        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            this.CreateLoot(itemLoot)
                .AddRouletteItem(ItemID.Spear)
                .AddRouletteItem(ItemID.Blowpipe)
                .AddRouletteItem(ItemID.WoodenBoomerang)
                .AddRouletteItem(ItemID.Aglet)
                .AddRouletteItem(ItemID.ClimbingClaws)
                .AddRouletteItem(ItemID.Umbrella)
                .AddRouletteItem(ItemID.WandofSparking)
                .AddRouletteItem(ItemID.Radar)
                .AddOptions(chance: 3, ItemID.HerbBag, ItemID.CanOfWorms)
                .Add(ItemID.SilverCoin, chance: 1, stack: (10, 40));
        }
    }
}