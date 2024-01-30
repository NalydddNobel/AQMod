using Terraria.ObjectData;

namespace Aequus.Old.Content.Tiles.Ambient;

public class GoreNestStalagmite : ModTile {
    public override void SetStaticDefaults() {
        Main.tileNoAttach[Type] = true;
        Main.tileFrameImportant[Type] = true;
        Main.tileNoFail[Type] = true;

        TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
        TileObjectData.newTile.StyleHorizontal = true;
        TileObjectData.newTile.WaterDeath = false;
        TileObjectData.newTile.LavaDeath = false;
        TileObjectData.newTile.CoordinateHeights = new System.Int32[2] { 16, 18 };
        TileObjectData.newTile.RandomStyleRange = 6;

        TileObjectData.addTile(Type);

        DustType = DustID.Blood;
        HitSound = SoundID.NPCDeath1;

        AddMapEntry(new Color(95, 22, 37, 255));
    }

    public override System.Boolean CreateDust(System.Int32 i, System.Int32 j, ref System.Int32 type) {
        type = DustType;
        Dust.NewDustDirect(new Vector2(i * 16f, j * 16f), 16, 16, DustID.FoodPiece, newColor: new Color(145, 10, 10)).velocity *= 2f;
        return true;
    }
}