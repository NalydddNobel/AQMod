using Aequus.Items.Placeable;
using Terraria;
using Terraria.ID;

namespace Aequus.Content.Biomes.CrabCrevice.Tiles {
    public class SedimentaryRock : FancyBlockItemBase<SedimentaryRockTile> {
        public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient(ItemID.SandBlock)
                .AddIngredient(ItemID.StoneBlock)
                .AddCondition(Condition.InBeach)
                .AddCondition(Condition.NearWater)
                .TryRegisterAfter(ItemID.ObsidianBackEcho);
            CreateRecipe()
                .AddIngredient<SedimentaryRockWall>(4)
                .AddTile(TileID.WorkBenches)
                .TryRegisterAfter(ItemID.ObsidianBackEcho);
        }
    }
}