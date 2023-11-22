using Aequus.Common.Tiles;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Tiles.Radon;

[LegacyName("RadonMossBrickWallPlaced")]
public class RadonMossBrickWall : ModWall {
    public override void Load() {
        Mod.AddContent(new InstancedWallItem(this));
    }

    public override void SetStaticDefaults() {
        DustType = DustID.Ambient_DarkBrown;
        AddMapEntry((Color.SaddleBrown * 0.66f) with { A = 255 });
    }
}