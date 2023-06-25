using Aequus.Common;
using Aequus.Items;
using System;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Unused.Items.SlotMachines {
    [Obsolete("Slot Machines were removed.")]
    [UnusedContent]
    public class ShadowRoulette : SlotMachineBase {
        public override void ModifyItemLoot(ItemLoot itemLoot) {
            this.CreateLoot(itemLoot)
                .AddRouletteItem(ItemID.Sunfury)
                .AddRouletteItem(ItemID.FlowerofFire)
                .AddRouletteItem(ItemID.Flamelash)
                .AddRouletteItem(ItemID.DarkLance)
                .AddRouletteItem(ItemID.HellwingBow)
                .AddRouletteItem(ItemID.TreasureMagnet)
                .AddRouletteItem(ItemID.HellMinecart, 4)
                .AddRouletteItem(ItemID.OrnateShadowKey, 4)
                .AddRouletteItem(ItemID.HellCake, 4);
            ModifyItemLoot_AddCommonDrops(itemLoot);
        }
    }
}