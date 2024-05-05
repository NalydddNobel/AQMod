using Aequus.Core.ContentGeneration;
using System.Collections.Generic;
using Terraria.Enums;

namespace Aequus.Content.Tiles.Meadow;
public class MeadowTree : UnifiedModTree {
    public override IEnumerable<int> ValidTiles() {
        yield return ModContent.TileType<MeadowGrass>();
    }

    public override int DropWood() {
        return ModContent.ItemType<MeadowWoodItem>();
    }

    public override void OnLoad() {
        _treeTypeOverride = TreeTypes.Forest;
    }

    public override bool Shake(int X, int Y, ref bool createLeaves) {
        return false;
    }
}
