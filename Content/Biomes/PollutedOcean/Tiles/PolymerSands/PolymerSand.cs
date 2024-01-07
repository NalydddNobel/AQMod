using Aequus.Common.Tiles;
using Aequus.Content.Biomes.PollutedOcean.Tiles.Scrap;

namespace Aequus.Content.Biomes.PollutedOcean.Tiles.PolymerSands;

public class PolymerSand : MultiMergeTile {
    public static ModItem Item { get; private set; }

    public override void Load() {
        Item = new InstancedTileItem(this).WithRecipe((item) => {
            item.CreateRecipe(5)
                .AddIngredient(ItemID.SandBlock, 5)
                .AddIngredient(ScrapBlock.Item)
                .AddTile(TileID.Furnaces)
                .Register();
        });
        Mod.AddContent(Item);
    }

    public override void SetStaticDefaults() {
        Main.tileSand[Type] = true;
        Main.tileSolid[Type] = true;
        Main.tileBlockLight[Type] = true;
        AddMerge(TileID.Sand);
        AddMerge(TileID.HardenedSand);
        Main.tileMerge[Type][ModContent.TileType<PolymerSandstone>()] = true;
        Main.tileMerge[ModContent.TileType<PolymerSandstone>()][Type] = true;

        TileID.Sets.ChecksForMerge[Type] = true;
        TileID.Sets.Conversion.Sand[Type] = true;
        AddMapEntry(new(117, 142, 154));
        DustType = DustID.Sand;
        HitSound = SoundID.Dig;
        MineResist = 1f;

        PollutedOceanSystem.BiomeTiles.Add(Type);
    }

    public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
}