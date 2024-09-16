#if POLLUTED_OCEAN
using Aequus.Common.ContentTemplates.Generic;

namespace Aequus.Content.Tiles.PollutedOcean.Scrap;

public class ScrapBrick : ModTile, IAddRecipes {
    public readonly ModItem Item;

    public ScrapBrick() {
        Item = new InstancedTileItem(this);
    }

    public override void Load() {
        Mod.AddContent(Item);
    }

    public override void SetStaticDefaults() {
        Main.tileSolid[Type] = true;
        Main.tileBlockLight[Type] = true;
        AddMapEntry(new(106, 82, 76));
        DustType = DustID.Tin;
        HitSound = SoundID.Tink;
        MineResist = 1.5f;
    }

    void IAddRecipes.AddRecipes() {
#if POLLUTED_OCEAN_TODO
            Item.CreateRecipe()
                .AddIngredient(Instance<ScrapBlock>().Item)
                .AddIngredient(ItemID.StoneBlock)
                .AddTile(TileID.Furnaces)
                .Register();
#endif
    }

    public override void NumDust(int i, int j, bool fail, ref int num) {
        num = fail ? 1 : 3;
    }
}
#endif