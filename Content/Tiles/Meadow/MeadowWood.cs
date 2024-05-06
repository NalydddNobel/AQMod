using Aequus.Core.ContentGeneration;
using Aequus.Core.Initialization;

namespace Aequus.Content.Tiles.Meadow;

internal class MeadowWood : ModTile, IAddRecipeGroups {
    public ModItem Item { get; private set; }

    public override void Load() {
        Item = new InstancedTileItem(this);
        Mod.AddContent(Item);
    }

    public override void SetStaticDefaults() {
        Main.tileSolid[Type] = true;
        Main.tileBlockLight[Type] = true;
        AddMapEntry(Color.LightGray);
        DustType = ModContent.GetInstance<MeadowTree>().DustType;

        ItemSets.ShimmerCountsAsItem[Item.Type] = ItemID.Wood;
    }

    public override void NumDust(int i, int j, bool fail, ref int num) {
        num = fail ? 1 : 3;
    }

    public void AddRecipeGroups(Aequus aequus) {
        RecipeGroup.recipeGroups[RecipeGroupID.Wood].ValidItems.Add(Item.Type);
    }
}
