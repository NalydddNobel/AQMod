using Aequus.Content.Items.Material;
using Microsoft.Xna.Framework;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Items.Misc.Dyes.Scorching {
    public class ScorchingDye : DyeItemBase {
        public override string Pass => "ScorchingPass";
        public override int Rarity => ItemRarityID.Orange;

        public override void SetStaticDefaults() {
            base.SetStaticDefaults();
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<AncientScorchingDye>();
        }

        public override ArmorShaderData CreateShaderData() {
            return base.CreateShaderData().UseImage(AequusTextures.EffectNoise).UseColor(new Color(140, 0, 21, 255));
        }

        public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient(ItemID.BottledWater)
                .AddIngredient<Fluorescence>()
                .AddTile(TileID.DyeVat)
                .Register();
        }
    }
}