﻿using Aequus.Common;
using Aequus.Items;
using System;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Unused.Items.SlotMachines {
    [Obsolete("Slot Machines were removed.")]
    [UnusedContent]
    public class DesertRoulette : SlotMachineBase {
        public override void ModifyItemLoot(ItemLoot itemLoot) {
            this.CreateLoot(itemLoot)
                .Add(ItemID.EncumberingStone, chance: 8, stack: 1)
                .Add(ItemID.Extractinator, chance: 5, stack: 1)
                .Add(ItemID.DesertMinecart, chance: 8, stack: 1)
                .Add(ItemID.SilverCoin, chance: 1, stack: (50, 80))
                .AddRouletteItem(ItemID.PharaohsMask)
                .AddSpecialRouletteItem(ItemID.PharaohsRobe, ItemID.PharaohsMask)
                .AddRouletteItem(ItemID.SandstorminaBottle)
                .AddRouletteItem(ItemID.FlyingCarpet)
                .AddRouletteItem(ItemID.AncientChisel)
                .AddRouletteItem(ItemID.SandBoots)
                .AddRouletteItem(ItemID.ThunderSpear)
                .AddRouletteItem(ItemID.ThunderStaff)
                .AddRouletteItem(ItemID.MagicConch, 4)
                .AddRouletteItem(ItemID.MysticCoilSnake)
                .AddRouletteItem(ItemID.CatBast, 4);
            ModifyItemLoot_AddCommonDrops(itemLoot);
        }
    }
}