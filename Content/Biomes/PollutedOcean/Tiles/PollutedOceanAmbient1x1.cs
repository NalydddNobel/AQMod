using Terraria.ObjectData;

namespace Aequus.Content.Biomes.PollutedOcean.Tiles;

public class PollutedOceanAmbient1x1 : ModTile {
    public override void SetStaticDefaults() {
        Main.tileFrameImportant[Type] = true;
        Main.tileNoAttach[Type] = true;
        Main.tileLavaDeath[Type] = true;
        Main.tileNoFail[Type] = true;

        TileID.Sets.DisableSmartCursor[Type] = true;

        TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
        TileObjectData.newTile.DrawYOffset = 2;
        TileObjectData.addTile(Type);

        HitSound = SoundID.Dig;

        AddMapEntry(new Color(80, 80, 80));
    }
}