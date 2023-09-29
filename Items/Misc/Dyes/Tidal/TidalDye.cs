using Aequus.Items.Material.Energy.Aquatic;
using Microsoft.Xna.Framework;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Misc.Dyes.Tidal {
    public class TidalDye : DyeItemBase {
        public override string Pass => "TidalPass";

        public override void SetStaticDefaults() {
            base.SetStaticDefaults();
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<AncientTidalDye>();
        }

        public override ArmorShaderData CreateShaderData() {
            return base.CreateShaderData().UseColor(new Vector3(0f, 0.3f, 1f));
        }

        public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient(ItemID.BottledWater)
                .AddIngredient<AquaticEnergy>()
                .AddTile(TileID.DyeVat)
                .Register();
        }
    }
}