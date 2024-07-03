using Aequu2.Core.ContentGeneration;

namespace Aequu2.Content.Tiles.PollutedOcean.Scrap;

public class ScrapBrick : ModTile {
    public override void Load() {
        ModItem item = new InstancedTileItem(this);
        Mod.AddContent(item);

        Aequu2.OnAddRecipes += () => {
            item.CreateRecipe()
                .AddIngredient(ScrapBlock.Item)
                .AddIngredient(ItemID.StoneBlock)
                .AddTile(TileID.Furnaces)
                .Register();
        };
    }

    public override void SetStaticDefaults() {
        Main.tileSolid[Type] = true;
        Main.tileBlockLight[Type] = true;
        AddMapEntry(new(106, 82, 76));
        DustType = DustID.Tin;
        HitSound = SoundID.Tink;
        MineResist = 1.5f;
    }

    public override void NumDust(int i, int j, bool fail, ref int num) {
        num = fail ? 1 : 3;
    }
}