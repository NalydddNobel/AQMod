using Terraria.Enums;
using Terraria.Localization;
using Terraria.ObjectData;

namespace Aequus.Content.Tiles.Statues;

public abstract class StatueTileTemplate : ModTile {
    protected virtual void SafeSetStaticDefaults() { }

    public sealed override void SetStaticDefaults() {
        Main.tileSpelunker[Type] = true;
        Main.tileFrameImportant[Type] = true;
        Main.tileObsidianKill[Type] = true;
        TileID.Sets.DisableSmartCursor[Type] = true;
        TileID.Sets.IsAMechanism[Type] = true;

        TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
        TileObjectData.newTile.Height = 3;
        TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 18 };
        TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
        TileObjectData.newTile.StyleWrapLimit = 2;
        TileObjectData.newTile.StyleMultiplier = 2;
        TileObjectData.newTile.StyleHorizontal = true;

        // Allows edits TileObjectData.newTile.
        SafeSetStaticDefaults();

        TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
        TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
        TileObjectData.addAlternate(1);

        TileObjectData.addTile(Type);

        DustType = DustID.Silver;

        AddMapEntry(CommonColor.MapStatue, Language.GetText("MapObject.Statue"));
    }
}
