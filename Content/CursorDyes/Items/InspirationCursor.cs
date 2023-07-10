using Aequus.Common.Items;
using Aequus.CrossMod;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.CursorDyes.Items {
    public class InspirationCursor : CursorDyeBase
    {
        public override void SetStaticDefaults() {
            base.SetStaticDefaults();
            Item.ResearchUnlockCount = ThoriumMod.Instance != null ? 1 : 0;
        }

        public override ICursorDye InitalizeDye()
        {
            return new ColorChangeCursor(() => Color.Lerp(Color.Turquoise * 1.35f, (Color.Cyan.HueMultiply(0.33f) * 0.33f).UseA(255), 1f - MathHelper.Clamp(GetInspirationPercent(Main.LocalPlayer), 0f, 1f)));
        }
        public float GetInspirationPercent(Player player)
        {
            if (ThoriumMod.Instance != null)
            {
                try
                {
                    int inspiration = (int)ThoriumMod.Instance.Call("GetBardInspiration", player);
                    int inspirationMax = (int)ThoriumMod.Instance.Call("GetBardInspirationMax", player);
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
            if (ThoriumMod.Instance == null)
            {
                tooltips.AddTooltip(new TooltipLine(Mod, "NeedsThorium", TextHelper.GetTextValue("NeedsMod", "Thorium Mod")) { OverrideColor = Color.Lerp(Color.White, Color.Turquoise * 1.5f, 0.5f), });
            }
        }

        public override void AddRecipes()
        {
            if (ThoriumMod.Instance != null)
            {
                int item = ItemID.DrumStick;
                if (ThoriumMod.Instance.TryFind<ModItem>("InspirationFragment", out var inspirationFragment))
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