using Aequus.Common.Tiles;
using Aequus.Content.Biomes.PollutedOcean.Tiles.Scrap;
using Terraria;
using Terraria.ID;

namespace Aequus.Content.Biomes.PollutedOcean.Tiles;

public class ScrapBrick : AequusModTile {
    public override void SetStaticDefaults() {
        Main.tileSolid[Type] = true;
        Main.tileBlockLight[Type] = true;
        AddMapEntry(new(106, 82, 76));
        DustType = DustID.Tin;
        HitSound = SoundID.Tink;
        MineResist = 1.5f;
    }

    public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;

    internal override void AddItemRecipes(AutoloadedTileItem modItem) {
        modItem.CreateRecipe()
            .AddIngredient<ScrapBlock>()
            .AddIngredient(ItemID.StoneBlock)
            .AddTile(TileID.Furnaces)
            .Register();
    }
}