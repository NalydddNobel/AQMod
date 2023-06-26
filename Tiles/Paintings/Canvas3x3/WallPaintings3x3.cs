using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Aequus.Tiles.Paintings.Canvas3x3;

[LegacyName("WallPaintings")]
public class WallPaintings3x3 : ModTile {
    public const int OriginPainting = 0;
    public const int RockFromAnAlternateUniversePainting = 1;
    public const int OmegaStaritePainting = 2;
    public const int GoreNestPainting = 3;
    public const int ExLydSpacePainting = 4;
    public const int GoreNest2Painting = 5;

    public override void SetStaticDefaults() {
        Main.tileFrameImportant[Type] = true;
        Main.tileLavaDeath[Type] = true;
        TileID.Sets.FramesOnKillWall[Type] = true;
        TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
        TileObjectData.newTile.StyleHorizontal = true;
        TileObjectData.newTile.StyleWrapLimit = 36;
        TileObjectData.addTile(Type);
        DustType = DustID.WoodFurniture;
        TileID.Sets.DisableSmartCursor[Type] = true;
        AddMapEntry(new Color(120, 85, 60), Language.GetText("MapObject.Painting"));
    }
}