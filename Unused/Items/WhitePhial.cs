using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Unused.Items {
    public class WhitePhial : ModItem {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 0;
        }

        public override void SetDefaults() {
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.width = 24;
            Item.height = 24;
            Item.consumable = true;
            Item.rare = ItemRarityID.Gray;
            Item.UseSound = SoundID.Item92;
            Item.value = Item.sellPrice(gold: 2);
            Item.maxStack = Item.CommonMaxStack;
        }

        public override void AddRecipes() {
            //AequusRecipes.AddShimmerCraft(ModContent.ItemType<BlackPhial>(), ModContent.ItemType<WhitePhial>(), condition: AequusConditions.DownedOmegaStarite);
        }
    }
}