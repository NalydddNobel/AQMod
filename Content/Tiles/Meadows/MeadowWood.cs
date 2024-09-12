using Aequus.Common.ContentTemplates.Generic;

namespace Aequus.Content.Tiles.Meadows;

internal class MeadowWood : ModTile, IAddRecipeGroups {
    public readonly ModItem Item;

    public MeadowWood() {
        Item = new InstancedTileItem(this);
    }

    public override void Load() {
        Mod.AddContent(Item);
    }

    public override void SetStaticDefaults() {
        Main.tileSolid[Type] = true;
        Main.tileBlockLight[Type] = true;
        AddMapEntry(Color.LightGray);
        DustType = ModContent.GetInstance<MeadowTree>().DustType;

        ItemID.Sets.ShimmerCountsAsItem[Item.Type] = ItemID.Wood;
    }

    public override void NumDust(int i, int j, bool fail, ref int num) {
        num = fail ? 1 : 3;
    }

    public void AddRecipeGroups(Aequus aequus) {
        RecipeGroup.recipeGroups[RecipeGroupID.Wood].ValidItems.Add(Item.Type);
    }
}
