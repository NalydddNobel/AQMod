using Aequus.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Misc.Energies
{
    public class UltimateEnergy : EnergyItemBase
    {
        public static StaticMiscShaderInfo EnergyShader;
        public static Asset<Texture2D> AuraTexture;
        public override ref StaticMiscShaderInfo Shader => ref EnergyShader;
        public override ref Asset<Texture2D> Aura => ref AuraTexture;
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