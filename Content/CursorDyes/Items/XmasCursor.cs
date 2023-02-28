using Aequus.Content.CursorDyes;
using Aequus.Items.Materials.Festive;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace Aequus.Content.CursorDyes.Items
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
                .AddIngredient<FrolicEnergy>(10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}