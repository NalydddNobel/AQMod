using Terraria.ObjectData;

namespace Aequus.Content.Biomes.PollutedOcean.Tiles;

public class PollutedOceanAmbient2x2 : ModTile {
    public override void SetStaticDefaults() {
        Main.tileFrameImportant[Type] = true;
        Main.tileNoAttach[Type] = true;
        Main.tileLavaDeath[Type] = true;
        Main.tileNoFail[Type] = true;

        TileID.Sets.DisableSmartCursor[Type] = true;

        TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
        TileObjectData.newTile.DrawYOffset = 2;
        TileObjectData.addTile(Type);

        HitSound = SoundID.Dig;

        AddMapEntry(new Color(100, 100, 100));
    }
}