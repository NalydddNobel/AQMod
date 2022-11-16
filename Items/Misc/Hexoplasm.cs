using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Misc
{
    public class Hexoplasm : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.SortingPriorityMaterials[Type] = ItemSortingPriority.Materials.Ectoplasm;
            SacrificeTotal = 25;
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
            CreateRecipe()
                .AddIngredient(ItemID.Ectoplasm)
                .AddIngredient(ItemID.SoulofNight)
                .AddTile(TileID.MythrilAnvil)
                .TryRegisterAfter(ItemID.SpectreBar);
            Recipe.Create(ItemID.Ectoplasm)
                .AddIngredient(Type)
                .AddIngredient(ItemID.SoulofLight)
                .AddTile(TileID.MythrilAnvil)
                .TryRegisterAfter(ItemID.SpectreBar);
        }
    }
}