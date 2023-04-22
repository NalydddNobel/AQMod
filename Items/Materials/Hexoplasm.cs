using Aequus.Common.Recipes;
using Aequus.Items.Tools;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Materials
{
    public class Hexoplasm : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.SortingPriorityMaterials[Type] = ItemSortingPriority.Materials.Ectoplasm;
            Item.ResearchUnlockCount = 25;
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.Ectoplasm);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(200, 200, 200, 0);
        }

        public override void AddRecipes()
        {
            AequusRecipes.AddShimmerCraft(Type, ItemID.Ectoplasm);
            AequusRecipes.AddShimmerCraft(ItemID.Ectoplasm, Type);
        }
    }
}