using Aequus.Common.Tiles;
using Aequus.Content.Biomes.PollutedOcean.Tiles.Scrap;
using Aequus.Core.ContentGeneration;
using Aequus.Core.Initialization;

namespace Aequus.Content.Biomes.PollutedOcean.Tiles.PolymerSands;

public class PolymerSand : MultiMergeTile {
    public static ModItem Item { get; private set; }

    public override void Load() {
        Item = new InstancedTileItem(this);
        Mod.AddContent(Item);

        LoadingSteps.EnqueueAddRecipes(() => {
            Item.CreateRecipe(5)
                .AddIngredient(ItemID.SandBlock, 5)
                .AddIngredient(ScrapBlock.Item)
                .AddTile(TileID.Furnaces)
                .Register();
        });
    }

    public override void SetStaticDefaults() {
        Main.tileSand[Type] = true;
        Main.tileSolid[Type] = true;
        Main.tileBlockLight[Type] = true;
        AddMerge(TileID.Sand);
        AddMerge(TileID.HardenedSand);
        AddMerge(ModContent.TileType<PolymerSandstone>());

        TileID.Sets.CanBeClearedDuringOreRunner[Type] = false;
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