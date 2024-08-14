using Aequus.Common.ContentTemplates;
using System.Collections.Generic;
using Terraria.Enums;
using Terraria.GameContent;

namespace Aequus.Content.Biomes.Meadows.Tiles;

public class MeadowTree : UnifiedModTree {
    public override IEnumerable<int> ValidTiles {
        get {
            yield return ModContent.TileType<MeadowGrass>();
        }
    }

    public override int DropWood() {
        return ModContent.GetInstance<MeadowWood>().Item.Type;
    }

    public override void OnLoad() {
        _treeTypeOverride = TreeTypes.Forest;
        _shaderSettings = new TreePaintingSettings {
            UseSpecialGroups = true,
            SpecialGroupMinimalHueValue = 0.5f,
            SpecialGroupMaximumHueValue = 0.6f,
            SpecialGroupMinimumSaturationValue = 0.6f,
            SpecialGroupMaximumSaturationValue = 0.75f
        };
    }

    public override bool Shake(int X, int Y, ref bool createLeaves) {
        return false;
    }
}
