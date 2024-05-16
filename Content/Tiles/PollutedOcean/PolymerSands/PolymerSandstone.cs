﻿using Aequus.Common.Tiles;
using Aequus.Core.ContentGeneration;
using Aequus.DataSets;

namespace Aequus.Content.Tiles.PollutedOcean.PolymerSands;

[LegacyName("SedimentaryRockTile", "SedimentaryRock")]
public class PolymerSandstone : MultiMergeTile {
    public override void Load() {
        ModItem item = new InstancedTileItem(this);
        Mod.AddContent(item);

        Aequus.OnAddRecipes += () => {
            item.CreateRecipe()
                .AddIngredient(PolymerSand.Item)
                .AddIngredient(ItemID.StoneBlock)
                .AddTile(TileID.Solidifier)
                .Register();
        };
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

        TileDataSet.Polluted.Add(Type);
        TileDataSet.GivesPollutedBiomePresence.Add(Type);
    }

    public override void NumDust(int i, int j, bool fail, ref int num) {
        num = fail ? 1 : 3;
    }
}