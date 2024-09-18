#if !CRAB_CREVICE_DISABLE
using Terraria.Enums;
using Terraria.ObjectData;

namespace Aequus.Tiles.CrabCrevice.Ambient;
public class CrabFloorPlants : ModTile {
    public override void SetStaticDefaults() {
        Main.tileNoAttach[Type] = true;
        Main.tileFrameImportant[Type] = true;
        Main.tileCut[Type] = true;

        TileID.Sets.SwaysInWindBasic[Type] = true;
        TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
        TileObjectData.newTile.Height = 1;
        TileObjectData.newTile.DrawYOffset = 1;
        TileObjectData.newTile.CoordinateHeights = new int[] { 32 };
        TileObjectData.newTile.StyleHorizontal = true;
        TileObjectData.newTile.WaterDeath = false;
        TileObjectData.newTile.LavaDeath = true;
        TileObjectData.newTile.WaterPlacement = LiquidPlacement.OnlyInFullLiquid;
        TileObjectData.newTile.RandomStyleRange = 16;

        TileObjectData.addTile(Type);

        DustType = DustID.JungleGrass;
        HitSound = SoundID.Grass;

        AddMapEntry((Color.YellowGreen * 0.8f).UseA(255).SaturationMultiply(0.5f));
    }

    public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY) {
        offsetY = -14;
        height = 32;
    }
}
#endif