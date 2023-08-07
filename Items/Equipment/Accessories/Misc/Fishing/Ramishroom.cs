using Aequus.Common.Recipes;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Equipment.Accessories.Misc.Fishing {
    public class Ramishroom : ModItem {
        public override void SetStaticDefaults() {
            AequusRecipes.AddShimmerCraft(Type, ModContent.ItemType<RegrowingBait>());
        }

        public override void SetDefaults() {
            Item.DefaultToAccessory(20, 20);
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(gold: 1);
        }

        public override void UpdateAccessory(Player player, bool hideVisual) {
            player.Aequus().accRamishroom = Item;
        }
    }
}