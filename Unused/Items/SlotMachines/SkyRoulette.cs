using Aequus.Items;
using System;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Unused.Items.SlotMachines {
    [Obsolete("Slot Machines were removed.")]
    public class SkyRoulette : SlotMachineBase {
        public override void ModifyItemLoot(ItemLoot itemLoot) {
            this.CreateLoot(itemLoot)
                .AddRouletteItem(ItemID.ShinyRedBalloon)
                .AddRouletteItem(ItemID.CreativeWings)
                .AddRouletteItem(ItemID.Starfury)
                .Add(ItemID.Cloud, chance: 2, stack: (15, 50))
                .Add(ItemID.SkyMill, chance: 2, stack: 1)
                .Add(ItemID.SilverCoin, chance: 1, stack: (50, 80));
            ModifyItemLoot_AddCommonDrops(itemLoot);
        }
    }
}