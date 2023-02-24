using Aequus.Content.CursorDyes;
using Aequus.Items.Materials.Festive;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace Aequus.Items.Accessories.Vanity.Cursors
{
    public class XmasCursor : CursorDyeBase
    {
        public override ICursorDye InitalizeDye()
        {
            return new TextureChangeCursor($"{Texture}/Cursor", new Vector2(0f, -2f));
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.HandWarmer)
                .AddIngredient(ItemID.SnowBlock, 150)
                .AddIngredient<XmasEnergy>(10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}