using Aequus.Graphics.ShaderData;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;

namespace Aequus.Items.Misc.Dyes.Ancient
{
    public class AncientFrostbiteDye : DyeItemBase
    {
        public override string Pass => "HoriztonalWavePass";
        public override int Rarity => ItemDefaults.RarityGaleStreams - 1;

        public override ArmorShaderData CreateShaderData()
        {
            return new ArmorShaderDataModifyLightColor(Effect, Pass, (v) =>
            {
                return v * new Vector3(0.05f, 0.2f, 0.9f);
            }).UseColor(new Vector3(0.05f, 0.2f, 0.9f)).UseSecondaryColor(new Vector3(0.1f, 0.4f, 2f));
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.BottledWater)
                .AddIngredient<FrozenTear>()
                .AddTile(TileID.DyeVat)
                .AddCondition(Recipe.Condition.InGraveyardBiome)
                .TryRegisterAfter(ItemID.FlameDye);
        }
    }
}