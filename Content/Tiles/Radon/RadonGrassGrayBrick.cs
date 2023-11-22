using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Tiles.Radon;

[LegacyName("RadonMossBrickTile")]
public class RadonGrassGrayBrick : RadonGrassStone {
    public override void SetStaticDefaults() {
        base.SetStaticDefaults();
        TileID.Sets.tileMossBrick[Type] = true;
        Main.tileMoss[Type] = false;
        RegisterItemDrop(ItemID.GrayBrick);
    }

    public override bool? PlaceTile(int i, int j, bool mute, bool forced, int plr, int style) {
        return null;
    }
}