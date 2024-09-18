using Terraria.ObjectData;

namespace Aequus.Items.Misc.GrabBags.Crates;
[LegacyName("FishingCrates")]
public class FishingCratesTile : ModTile {
    public const int CrabCreviceCrate = 0;
    public const int CrabCreviceCrateHard = 1;

    public override void SetStaticDefaults() {
        Main.tileFrameImportant[Type] = true;
        Main.tileSolidTop[Type] = true;
        Main.tileTable[Type] = true;

        TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
        TileObjectData.newTile.CoordinateHeights = new int[2] { 16, 18 };
        TileObjectData.newTile.StyleHorizontal = true;
        TileObjectData.addTile(Type);

        AddMapEntry(Color.Brown * 1.5f, CreateMapEntryName());
    }

    public override bool CreateDust(int i, int j, ref int type) {
        return false;
    }
}