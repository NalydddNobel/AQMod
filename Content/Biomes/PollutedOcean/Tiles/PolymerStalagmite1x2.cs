using Terraria.ObjectData;

namespace Aequus.Content.Biomes.PollutedOcean.Tiles;

public class PolymerStalagmite1x2 : ModTile {
    public override void SetStaticDefaults() {
        Main.tileSolid[Type] = false;
        Main.tileSolidTop[Type] = false;
        Main.tileNoAttach[Type] = true;
        Main.tileFrameImportant[Type] = true;

        TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
        TileObjectData.newTile.StyleHorizontal = true;
        TileObjectData.newTile.WaterDeath = false;
        TileObjectData.newTile.LavaDeath = false;
        TileObjectData.newTile.DrawYOffset = 4;
        TileObjectData.addTile(Type);

        HitSound = SoundID.Dig;

        AddMapEntry(new(130, 119, 77));
    }
}
