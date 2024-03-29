﻿using Aequus.Common.DataSets;
using Aequus.Common.Items;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Misc.Foods.Baguette {
    public class Baguette : ModItem {
        public override void SetStaticDefaults() {
            this.StaticDefaultsToFood(new Color(194, 136, 36, 255), new Color(147, 103, 27, 255), new Color(100, 49, 2, 255));
            ItemSets.DedicatedContent[Type] = new("niker", new Color(187, 142, 42, 255)) { UseAnonymousText = true, };
        }

        public override void SetDefaults() {
            Item.DefaultToFood(20, 20, ModContent.BuffType<BaguetteBuff>(), 216000);
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.buyPrice(gold: 10);
            Item.maxStack = 29;
        }
    }
}