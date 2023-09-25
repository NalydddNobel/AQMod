using Aequus.Items.Material.OmniGem;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;

namespace Aequus.Items.Misc.Dyes;

public class HueshiftDye : DyeItemBase {
    public override int Rarity => ItemRarityID.Green;

    public override string Pass => "HueShiftPass";

    public override ArmorShaderData CreateShaderData() {
        return new ArmorShaderData(Effect, Pass).UseOpacity(1f);
    }

    public override void AddRecipes() {
        CreateRecipe()
            .AddIngredient(ItemID.BottledWater)
            .AddIngredient<OmniGem>()
            .AddTile(TileID.DyeVat)
            .Register();
    }
}