﻿using Aequus.Items;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.NPCs.Boss.SpaceSquidMiniboss.Rewards {
    [AutoloadEquip(EquipType.Head)]
    public class SpaceSquidMask : ModItem {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 1;
            ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true;
        }

        public override void SetDefaults() {
            Item.DefaultToHeadgear(16, 16, Item.headSlot);
            Item.rare = ItemDefaults.RarityBossMasks;
            Item.vanity = true;
            Item.Aequus().itemGravityCheck = 255;
        }
    }
}