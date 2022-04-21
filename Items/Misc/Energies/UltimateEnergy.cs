using Aequus.Common.Utilities;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Misc.Energies
{
    public class UltimateEnergy : BaseEnergy
    {
        protected override IColorGradient Gradient => ColorHelper.Instance.UltimateGrad;
        protected override Vector3 LightColor => new Vector3(0.5f, 0.5f, 0.5f);
        public override int Rarity => ItemRarityID.Pink;

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<AquaticEnergy>())
                .AddIngredient(ModContent.ItemType<OrganicEnergy>())
                .AddIngredient(ModContent.ItemType<AtmosphericEnergy>())
                .AddIngredient(ModContent.ItemType<DemonicEnergy>())
                .AddIngredient(ModContent.ItemType<CosmicEnergy>())
                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }
}