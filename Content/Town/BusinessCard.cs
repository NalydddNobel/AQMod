using Aequus.Common.Recipes;
using Aequus.Items;
using Aequus.Items.Accessories.Misc;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Town {

    [LegacyName("ForgedCard")]
    public class BusinessCard : ModItem {

        public override void SetStaticDefaults() {
            SacrificeTotal = 1;
            AequusItem.AddTooltipModifier(Type, new TooltipModifierNoBoostInteractions());
        }

        public override void SetDefaults() {
            Item.DefaultToAccessory();
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 50);
            Item.canBePlacedInVanityRegardlessOfConditions = true;
        }

        public void UpdateEffect(Player player) {
            var aequus = player.Aequus();
            aequus.increasedSellPrice = Math.Max(aequus.increasedSellPrice, 0.05);
        }

        public override void UpdateAccessory(Player player, bool hideVisual) {
            UpdateEffect(player);
        }

        public override void UpdateInventory(Player player) {
            UpdateEffect(player);
        }

        public override void AddRecipes() {
            AequusRecipes.CreateShimmerTransmutation(Type, ModContent.ItemType<FaultyCoin>());
        }
    }
}

namespace Aequus {
    public partial class AequusPlayer {
        public double increasedSellPrice;
    }
}