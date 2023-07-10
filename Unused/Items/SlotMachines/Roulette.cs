using Aequus.Common;
using Aequus.CrossMod;
using Aequus.Items;
using System;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Unused.Items.SlotMachines {
    [Obsolete("Slot Machines were removed.")]
    [UnusedContent]
    public class Roulette : SlotMachineBase {
        public override void ModifyItemLoot(ItemLoot itemLoot) {
            var builder = this.CreateLoot(itemLoot);
            builder.AddRouletteItem(ItemID.Spear)
                .AddRouletteItem(ItemID.Blowpipe)
                .AddRouletteItem(ItemID.WoodenBoomerang)
                .AddRouletteItem(ItemID.Aglet)
                .AddRouletteItem(ItemID.ClimbingClaws)
                .AddRouletteItem(ItemID.Umbrella)
                .AddRouletteItem(ItemID.WandofSparking)
                .AddRouletteItem(ItemID.Radar)
                .AddOptions(chance: 3, ItemID.HerbBag, ItemID.CanOfWorms)
                .Add(ItemID.SilverCoin, chance: 1, stack: (10, 40));
            if (ThoriumMod.Instance != null) {
                if (ThoriumMod.Instance.TryFind("RecoveryWand", out ModItem modItem))
                    builder.AddRouletteItem(modItem.Type);
                if (ThoriumMod.Instance.TryFind("Flute", out modItem))
                    builder.AddRouletteItem(modItem.Type);
            }
        }
    }
}