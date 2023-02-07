using Aequus.Items.Accessories.Debuff;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Consumables.Permanent
{
    public class WhitePhial : ModItem
    {
        public override void SetDefaults()
        {
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.width = 24;
            Item.height = 24;
            Item.consumable = true;
            Item.rare = ItemRarityID.LightPurple;
            Item.UseSound = SoundID.Item92;
            Item.value = Item.sellPrice(gold: 2);
            Item.maxStack = 9999;
        }

        public override bool? UseItem(Player player)
        {
            if (!player.Aequus().whitePhial)
            {
                player.Aequus().whitePhial = true;
                return true;
            }

            return false;
        }

        public override void AddRecipes()
        {
            AequusRecipes.CreateShimmerTransmutation(ModContent.ItemType<BlackPhial>(), ModContent.ItemType<WhitePhial>(), condition: AequusRecipes.ConditionOmegaStarite);
        }
    }
}