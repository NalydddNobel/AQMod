using Aequus.Common.Tiles;
using Terraria.Localization;
using Terraria.ObjectData;

namespace Aequus.Content.Bosses;

internal class InstancedTrophyTile : InstancedModTile {
    public InstancedTrophyTile(System.String name) : base($"{name}Trophy", $"{typeof(InstancedTrophyTile).NamespaceFilePath()}/Trophies/{name}Trophy") {
    }

    public override void SetStaticDefaults() {
        Main.tileSpelunker[Type] = true;
        Main.tileFrameImportant[Type] = true;
        Main.tileLavaDeath[Type] = true;
        TileID.Sets.FramesOnKillWall[Type] = true;
        TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
        TileObjectData.newTile.StyleHorizontal = true;
        TileObjectData.newTile.StyleWrapLimit = 36;
        TileObjectData.addTile(Type);
        DustType = DustID.WoodFurniture;
        TileID.Sets.DisableSmartCursor[Type] = true;
        AddMapEntry(new(120, 85, 60), Language.GetText("MapObject.Trophy"));
    }
}