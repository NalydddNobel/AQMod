using Terraria.ObjectData;

namespace AequusRemake.Content.Tiles.PollutedOcean.Ambient.Dripstones;

public class PolymerStalagmite1x1 : ModTile {
    public override void SetStaticDefaults() {
        Main.tileSolid[Type] = false;
        Main.tileSolidTop[Type] = false;
        Main.tileNoAttach[Type] = true;
        Main.tileFrameImportant[Type] = true;

        TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
        TileObjectData.newTile.StyleHorizontal = true;
        TileObjectData.newTile.WaterDeath = false;
        TileObjectData.newTile.LavaDeath = false;
        TileObjectData.newTile.DrawYOffset = 4;
        TileObjectData.addTile(Type);

        HitSound = SoundID.Dig;

        AddMapEntry(new(130, 119, 77));
    }
}
