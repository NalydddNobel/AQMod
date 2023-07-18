using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Equipment.PetsVanity.MechPetCombo {
    public class TheReconstruction : ModItem {
        public override void SetDefaults() {
            // TODO: Add a custom projectile ID to render all 3 pets in the player selection menu?
            Item.DefaultToVanitypet(ProjectileID.SkeletronPrimePet, ModContent.BuffType<MechPetComboBuff>());
            Item.width = 20;
            Item.height = 20;
            Item.value = Item.sellPrice(gold: 15);
            Item.master = true;
        }

        public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient(ItemID.TwinsPetItem)
                .AddIngredient(ItemID.DestroyerPetItem)
                .AddIngredient(ItemID.SkeletronPrimePetItem)
                .TryRegisterAfter(ItemID.ResplendentDessert);
        }
    }
}