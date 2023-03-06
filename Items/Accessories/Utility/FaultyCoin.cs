using Aequus.Common.Recipes;
using Aequus.Items.Accessories.Offense;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Utility
{
    public class FaultyCoin : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToAccessory();
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(gold: 10);
            Item.hasVanityEffects = true;
        }

        public override void UpdateEquip(Player player)
        {
            player.Aequus().accFaultyCoin += 0.1f;
        }

        public override void AddRecipes()
        {
            AequusRecipes.CreateShimmerTransmutation(Type, ModContent.ItemType<FoolsGoldRing>());
        }
    }
}