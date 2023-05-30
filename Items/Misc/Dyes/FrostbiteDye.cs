using Aequus.Common.Effects.ShaderData;
using Aequus.Items.Materials;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Misc.Dyes {
    public class FrostbiteDye : DyeItemBase
    {
        public override Ref<Effect> Effect => FromAssetFolder("Dyes/FrostbiteDyeShader");
        public override string Pass => "FrostbitePass";
        public override int Rarity => ItemRarityID.Orange;

        public override ArmorShaderData CreateShaderData()
        {
            return new ArmorCustomTexture(Effect, Pass, ModContent.Request<Texture2D>($"{Texture}Effect"));
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.BottledWater)
                .AddIngredient<FrozenTear>()
                .AddTile(TileID.DyeVat)
                .TryRegisterAfter(ItemID.FlameDye);
        }
    }
}