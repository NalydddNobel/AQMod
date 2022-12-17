using Aequus.Content.CursorDyes;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Vanity.Cursors
{
    [LegacyName("ManaCursorDye")]
    public class ManaCursor : CursorDyeBase
    {
        public override ICursorDye InitalizeDye()
        {
            return new ColorChangeCursor(() => Color.Lerp(Color.White, Color.Blue, MathHelper.Clamp(Main.LocalPlayer.statMana / (float)Main.LocalPlayer.statManaMax2, 0f, 1f)));
        }
    }
}