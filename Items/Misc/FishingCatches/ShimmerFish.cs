using Aequus.Common.Recipes;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Misc.FishingCatches {
    public class ShimmerFish : ModItem {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 2;
        }

        public override void SetDefaults() {
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.width = 24;
            Item.height = 24;
            Item.consumable = true;
            Item.rare = ItemRarityID.LightPurple;
            Item.UseSound = SoundID.Item2;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.sellPrice(gold: 2);
        }

        public override bool? UseItem(Player player) {
            if (Main.netMode != NetmodeID.MultiplayerClient) {
                Main.AnglerQuestSwap();
                NetMessage.SendData(MessageID.AnglerQuest, number: Main.anglerQuest);
            }
            return true;
        }

        public override void AddRecipes() {
            AequusRecipes.AddShimmerCraft(AequusRecipes.AnyQuestFish, ModContent.ItemType<ShimmerFish>());
        }
    }
}