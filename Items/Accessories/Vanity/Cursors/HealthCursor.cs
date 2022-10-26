using Aequus.Content.CursorDyes;
using Aequus.Items.Misc.Energies;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Vanity.Cursors
{
    [LegacyName("HealthCursorDye")]
    public class HealthCursor : CursorDyeBase
    {
        public override ICursorDye InitalizeDye()
        {
            return new ColorChangeCursor(() => Color.Lerp(Color.Black, Color.Red, MathHelper.Clamp(Main.LocalPlayer.statLife / (float)Main.LocalPlayer.statLifeMax2, 0f, 1f)));
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<DyableCursor>()
                .AddIngredient(ItemID.LifeCrystal)
                .Register();
        }
    }
}