﻿using Aequus.Items.Accessories.CrownOfBlood;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Misc.Info {
    public class AnglerBroadcaster : ModItem {
        public override void SetStaticDefaults() {
            CrownOfBloodItem.NoBoost.Add(Type);
        }

        public override void SetDefaults() {
            Item.CloneDefaults(ItemID.LifeformAnalyzer);
        }

        public override void UpdateInfoAccessory(Player player) {
            player.Aequus().accShowQuestFish = true;
        }
    }
}