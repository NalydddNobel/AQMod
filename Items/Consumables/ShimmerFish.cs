using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Consumables
{
    public class ShimmerFish : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 2;
        }

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.width = 24;
            Item.height = 24;
            Item.consumable = true;
            Item.rare = ItemRarityID.LightPurple;
            Item.UseSound = SoundID.Item2;
            Item.maxStack = 9999;
            Item.value = Item.sellPrice(gold: 2);
        }

        public override bool? UseItem(Player player)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Main.AnglerQuestSwap();
            }
            return true;
        }

        public override void AddRecipes()
        {
            AequusRecipes.CreateShimmerTransmutation(AequusRecipes.AnyQuestFish, ModContent.ItemType<ShimmerFish>(), condition: AequusRecipes.ConditionOmegaStarite);
        }
    }
}