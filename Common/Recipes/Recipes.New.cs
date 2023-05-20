using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Common.Recipes {
    public partial class AequusRecipes : ModSystem {
        public override void AddRecipes() {
            Recipe.Create(ItemID.FlyingCarpet)
                .AddIngredient(ItemID.Silk, 12)
                .AddIngredient(ItemID.AncientBattleArmorMaterial);
        }
    }
}