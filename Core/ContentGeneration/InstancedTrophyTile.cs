using Aequus.Content.Bosses;
using Terraria.Localization;
using Terraria.ObjectData;

namespace Aequus.Core.ContentGeneration;

internal class InstancedTrophyTile : InstancedModTile {
    public InstancedTrophyTile(ModNPC modNPC) : base($"{modNPC.Name}Trophy", $"{modNPC.NamespaceFilePath()}/Items/{modNPC.Name}Trophy") { }
    public InstancedTrophyTile(string name) : base($"{name}Trophy", $"{typeof(BossItemInstantiator).NamespaceFilePath()}/Trophies/{name}Trophy") { }

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
        AddMapEntry(new Color(120, 85, 60), Language.GetText("MapObject.Trophy"));
    }
}