using Aequus.Common.Recipes;
using Aequus.Items.Accessories.Utility;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Misc {

    [LegacyName("ForgedCard")]
    public class BusinessCard : ModItem {
        public override void SetStaticDefaults() {
            SacrificeTotal = 1;
        }

        public override void SetDefaults() {
            Item.DefaultToAccessory();
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 50);
            Item.canBePlacedInVanityRegardlessOfConditions = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual) {
            player.Aequus().accForgedCard += Item.gold;
            if (Main.myPlayer == player.whoAmI) {
                Main.shopSellbackHelper.Clear();
            }
        }

        public override void AddRecipes() {
            AequusRecipes.CreateShimmerTransmutation(Type, ModContent.ItemType<FaultyCoin>());
        }
    }
}