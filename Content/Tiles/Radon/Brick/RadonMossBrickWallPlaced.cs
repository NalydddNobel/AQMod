using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Tiles.Radon.Brick;

public class RadonMossBrickWallPlaced : ModWall {
    public override void SetStaticDefaults() {
        DustType = DustID.Ambient_DarkBrown;
        AddMapEntry((Color.SaddleBrown * 0.66f) with { A = 255 });
    }
}