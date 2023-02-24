using Aequus.Content.CursorDyes;
using Aequus.Items.Materials.Festive;
using Terraria.ID;

namespace Aequus.Items.Accessories.Vanity.Cursors
{
    public class PumpkingCursor : CursorDyeBase
    {
        public override ICursorDye InitalizeDye()
        {
            return new TextureChangeCursor($"{Texture}/Cursor");
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Sickle)
                .AddIngredient(ItemID.Pumpkin, 150)
                .AddIngredient<HalloweenEnergy>(10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}