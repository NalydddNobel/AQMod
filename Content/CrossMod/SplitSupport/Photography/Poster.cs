using Terraria.DataStructures;
using Terraria.ObjectData;

namespace Aequus.Content.CrossMod.SplitSupport.Photography;

[LegacyName("PrintsTile")]
public class Poster : ModTile {
    public override void SetStaticDefaults() {
        Main.tileFrameImportant[Type] = true;
        TileID.Sets.FramesOnKillWall[Type] = true;
        Main.tileLavaDeath[Type] = true;
        TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
        TileObjectData.newTile.Origin = new Point16(3, 3);
        TileObjectData.newTile.Height = 6;
        TileObjectData.newTile.Width = 6;
        TileObjectData.newTile.CoordinateHeights = new int[]
        {
            16, 16, 16, 16, 16, 16
        };
        TileObjectData.newTile.StyleHorizontal = true;
        TileObjectData.newTile.StyleWrapLimit = 6;
        TileObjectData.addTile(Type);
        DustType = -1;
        AddMapEntry(CommonColor.TILE_FURNITURE);
    }

    public override void NumDust(int i, int j, bool fail, ref int num) {
        num = 0;
    }
}