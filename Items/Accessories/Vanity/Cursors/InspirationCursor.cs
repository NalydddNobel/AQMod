using Aequus.Content.CrossMod;
using Aequus.Content.CursorDyes;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Vanity.Cursors
{
    public class InspirationCursor : CursorDyeBase
    {
        public override ICursorDye InitalizeDye()
        {
            return new ColorChangeCursor(() => Color.Lerp(Color.Turquoise * 1.35f, (Color.Cyan.HueMultiply(0.33f) * 0.33f).UseA(255), 1f - MathHelper.Clamp(GetInspirationPercent(Main.LocalPlayer), 0f, 1f)));
        }
        public float GetInspirationPercent(Player player)
        {
            if (ThoriumModSupport.ThoriumMod != null)
            {
                try
                {
                    int inspiration = (int)ThoriumModSupport.ThoriumMod.Call("GetBardInspiration", player);
                    int inspirationMax = (int)ThoriumModSupport.ThoriumMod.Call("GetBardInspirationMax", player);
                    return inspiration / (float)inspirationMax;
                }
                catch
                {
                }
            }
            return 1f;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (ThoriumModSupport.ThoriumMod == null)
            {
                tooltips.AddTooltip(new TooltipLine(Mod, "NeedsThorium", AequusText.GetText("NeedsMod", "Thorium Mod")) { OverrideColor = Color.Lerp(Color.White, Color.Turquoise * 1.5f, 0.5f), });
            }
        }

        public override void AddRecipes()
        {
            if (ThoriumModSupport.ThoriumMod != null)
            {
                int item = ItemID.DrumStick;
                if (ThoriumModSupport.ThoriumMod.TryFind<ModItem>("InspirationFragment", out var inspirationFragment))
                {
                    item = inspirationFragment.Type;
                }
                CreateRecipe()
                    .AddIngredient<DyableCursor>()
                    .AddIngredient(item)
                    .Register();
            }
        }
    }
}