using Aequus.Common.Items;
using Aequus.Items.Material.Energy.Atmospheric;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Misc.Dyes.Gust;

public class GustDye : DyeItemBase {
    public override string Pass => "GustPass";
    public override int Rarity => ItemCommons.Rarity.SpaceStormLoot;

    public override ArmorShaderData CreateShaderData() {
        return new ArmorShaderData(Effect, Pass).UseImage(AequusTextures.EffectPerlin).UseOpacity(0.8f).UseColor(new Vector3(1f, 1f, 0.33f));
    }

    public override void AddRecipes() {
        CreateRecipe()
            .AddIngredient(ItemID.BottledWater)
            .AddIngredient<AtmosphericEnergy>()
            .AddTile(TileID.DyeVat)
            .Register();
    }
}