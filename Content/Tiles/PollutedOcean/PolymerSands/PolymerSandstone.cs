#if POLLUTED_OCEAN
using Aequus.Common.ContentTemplates.Generic;
using Aequus.Common.ContentTemplates.Tiles;
using Aequus.Content.Biomes.PollutedOcean;

namespace Aequus.Content.Tiles.PollutedOcean.PolymerSands;

#if CRAB_CREVICE_DISABLE
[LegacyName("SedimentaryRockTile")]
#endif
public class PolymerSandstone : MultiMergeTile, IAddRecipes {
    public readonly ModItem Item;

    public PolymerSandstone() {
        Item = new InstancedTileItem(this);
    }

    public override void Load() {
        Mod.AddContent(Item);
    }

    public override void SetStaticDefaults() {
        Main.tileSolid[Type] = true;
        Main.tileBlockLight[Type] = true;
        AddMerge(TileID.Sand);
        AddMerge(TileID.HardenedSand);

        TileID.Sets.CanBeClearedDuringOreRunner[Type] = true;
        TileID.Sets.ChecksForMerge[Type] = true;
        TileID.Sets.Conversion.Sandstone[Type] = true;
        AddMapEntry(new(160, 149, 97));
        DustType = DustID.Sand;
        HitSound = SoundID.Tink;
        MineResist = 1.1f;

        Instance<PollutedOceanSystem>().IsPolluted.Add(Type);
    }

    void IAddRecipes.AddRecipes() {
        Item.CreateRecipe()
            .AddIngredient(Instance<PolymerSand>().Item)
            .AddIngredient(ItemID.StoneBlock)
            .AddTile(TileID.Solidifier)
            .Register();
    }

    public override void NumDust(int i, int j, bool fail, ref int num) {
        num = fail ? 1 : 3;
    }
}
#endif