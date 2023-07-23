using Aequus.Common;
using Aequus.Content.CursorDyes;
using Aequus.Content.CursorDyes.Items;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.CrossMod.ThoriumModSupport.ItemContent {
    [ModRequired("ThoriumMod")]
    public class InspirationCursor : CursorDyeBase {
        public override void SetStaticDefaults() {
            base.SetStaticDefaults();
        }

        public override ICursorDye InitalizeDye() {
            return new ColorChangeCursor(() => Color.Lerp(Color.Turquoise * 1.35f, (Color.Cyan.HueMultiply(0.33f) * 0.33f).UseA(255), 1f - MathHelper.Clamp(GetInspirationPercent(Main.LocalPlayer), 0f, 1f)));
        }
        public float GetInspirationPercent(Player player) {
            if (ThoriumMod.Instance != null) {
                try {
                    int inspiration = (int)ThoriumMod.Instance.Call("GetBardInspiration", player);
                    int inspirationMax = (int)ThoriumMod.Instance.Call("GetBardInspirationMax", player);
                    return inspiration / (float)inspirationMax;
                }
                catch {
                }
            }
            return 1f;
        }

        public override void AddRecipes() {
            if (ThoriumMod.Instance != null) {
                int item = ItemID.DrumStick;
                if (ThoriumMod.Instance.TryFind<ModItem>("InspirationFragment", out var inspirationFragment)) {
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