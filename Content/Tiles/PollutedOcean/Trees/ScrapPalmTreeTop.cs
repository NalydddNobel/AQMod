#if POLLUTED_OCEAN_TODO
using Aequus.Common.ContentTemplates.Generic;
using Aequus.Core.ContentGeneration;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.Localization;
using Terraria.ObjectData;

namespace Aequus.Content.Tiles.PollutedOcean.Trees;

internal class ScrapPalmTreeTop(ScrapPalmTree parent) : InstancedModTile($"{parent.Name}Top", $"{parent.Texture}Top") {
    public ModItem DropItem { get; protected set; }

    public override void Load() {
        DropItem = new InstancedTileItem(this);
        Mod.AddContent(DropItem);
    }

    public override void SetStaticDefaults() {
        Main.tileFrameImportant[Type] = true;
        TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
        TileObjectData.newTile.CoordinateWidth = 100;
        TileObjectData.newTile.CoordinateHeights = [76];
        TileObjectData.newTile.DrawYOffset = -60;
        TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.AlternateTile, TileObjectData.newTile.Width, 0);
        TileObjectData.newTile.AnchorAlternateTiles = [parent.Type];
        TileObjectData.newTile.StyleHorizontal = true;
        TileObjectData.newTile.StyleWrapLimit = 7;
        TileObjectData.newTile.RandomStyleRange = 3;

        TileObjectData.addTile(Type);

        MineResist = 8f;
        DustType = DustID.Iron;
        AddMapEntry(new Color(192, 187, 151), Language.GetText("MapObject.PalmTree"));
    }

    public override void NumDust(int i, int j, bool fail, ref int num) {
        num = fail ? 1 : 3;
    }
}
#endif