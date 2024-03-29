﻿using Aequus.Common;
using Aequus.Common.DataSets;
using Aequus.Common.Items;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items.Equipment.Accessories.Necro {
    [WorkInProgress]
    public class PandorasBox : ModItem {
        public override void SetStaticDefaults() {
#if DEBUG
            ChestLootDataset.AequusDungeonChestLoot.Add(Type);
#endif
        }

        public override void SetDefaults() {
            Item.width = 24;
            Item.height = 24;
            Item.accessory = true;
            Item.rare = ItemDefaults.RarityDungeon;
            Item.value = ItemDefaults.ValueDungeon;
        }

        public override void UpdateAccessory(Player player, bool hideVisual) {
            var aequus = player.Aequus();
            aequus.zombieDebuffMultiplier++;
            aequus.ghostProjExtraUpdates += 1;
        }
    }
}