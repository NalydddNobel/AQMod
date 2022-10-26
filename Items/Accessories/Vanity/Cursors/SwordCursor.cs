using Aequus.Content.CursorDyes;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Vanity.Cursors
{
    [LegacyName("SwordCursorDye")]
    public class SwordCursor : CursorDyeBase
    {
        public override ICursorDye InitalizeDye()
        {
            return new TextureChangeCursor($"{Texture}/SwordCursor");
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<DyableCursor>()
                .AddIngredient(ItemID.Diamond, 2)
                .Register();
        }
    }
}