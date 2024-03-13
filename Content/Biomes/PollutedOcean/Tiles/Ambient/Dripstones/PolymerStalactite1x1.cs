using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ObjectData;

namespace Aequus.Content.Biomes.PollutedOcean.Tiles.Ambient.Dripstones;

public class PolymerStalactite1x1 : ModTile {
    public override void SetStaticDefaults() {
        Main.tileSolid[Type] = false;
        Main.tileSolidTop[Type] = false;
        Main.tileNoAttach[Type] = true;
        Main.tileFrameImportant[Type] = true;

        TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
        TileObjectData.newTile.StyleHorizontal = true;
        TileObjectData.newTile.WaterDeath = false;
        TileObjectData.newTile.LavaDeath = false;
        TileObjectData.newTile.DrawYOffset = -4;
        TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, 1, 1);
        TileObjectData.newTile.AnchorBottom = AnchorData.Empty;
        TileObjectData.newTile.Origin = new Point16(0, 0);

        TileObjectData.addTile(Type);

        HitSound = SoundID.Dig;

        AddMapEntry(new(130, 119, 77));
    }
}
