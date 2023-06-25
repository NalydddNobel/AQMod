using Aequus.Common;
using Aequus.Items;
using System;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Unused.Items.SlotMachines {
    [Obsolete("Slot Machines were removed.")]
    [UnusedContent]
    public class OceanSlotMachine : SlotMachineBase {
        public override void ModifyItemLoot(ItemLoot itemLoot) {
            this.CreateLoot(itemLoot)
                .AddRouletteItem(ItemID.BreathingReed)
                .AddRouletteItem(ItemID.Flipper)
                .AddRouletteItem(ItemID.Trident)
                .AddRouletteItem(ItemID.FloatingTube)
                .AddRouletteItem(ItemID.WaterWalkingBoots)
                .AddRouletteItem(ItemID.BeachBall)
                .AddRouletteItem(ItemID.SharkBait)
                .AddRouletteItem(ItemID.SandcastleBucket, 4)
                .Add(ItemID.GillsPotion, chance: 1, stack: (1, 2))
                .Add(ItemID.SilverCoin, chance: 1, stack: (50, 80));
            ModifyItemLoot_AddCommonDrops(itemLoot);
        }
    }
}