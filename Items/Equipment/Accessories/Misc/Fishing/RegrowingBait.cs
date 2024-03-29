﻿using Aequus.Common.Items.EquipmentBooster;
using Aequus.Common.Recipes;
using Aequus.Items.Misc.FishingBait;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Equipment.Accessories.Misc.Fishing {
    public class RegrowingBait : ModItem {
        public override void SetStaticDefaults() {
            EquipBoostDatabase.Instance.SetNoEffect(Type);
            AequusRecipes.AddShimmerCraft(Type, ModContent.ItemType<NeonGenesis.NeonGenesis>());
        }

        public override void SetDefaults() {
            Item.width = 16;
            Item.height = 16;
            Item.accessory = true;
            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ItemRarityID.Orange;
        }

        public override bool? CanConsumeBait(Player player) {
            return Item.consumable ? null : false;
        }

        public override void UpdateAccessory(Player player, bool hideVisual) {
            CheckRegrowingBait(player, Item);
        }

        public static void CheckRegrowingBait(Player player, Item me) {
            int amt = 10;
            for (int i = 0; i < Main.InventorySlotsTotal; i++) {
                if (player.inventory[i].bait >= 25) {
                    amt -= player.inventory[i].stack;
                    if (amt <= 0)
                        return;
                }
            }
            if (Main.myPlayer == player.whoAmI && Main.mouseItem?.IsAir == false && Main.mouseItem.bait > 0) {
                amt -= Main.mouseItem.stack;
                if (amt <= 0)
                    return;
            }
            var item = AequusItem.SetDefaults<XenonBait>();
            item.stack = amt;
            var itemSpace = player.ItemSpace(item);
            if (!itemSpace.CanTakeItem || itemSpace.ItemIsGoingToVoidVault || Main.myPlayer != player.whoAmI) {
                return;
            }
            player.QuickSpawnItem(player.GetSource_Accessory(me), item, amt);
        }
    }
}