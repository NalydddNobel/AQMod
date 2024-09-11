using Terraria.GameContent;
using Terraria.ObjectData;

namespace Aequus.Common.ContentTemplates.Tiles.Rubblemaker;

internal abstract class Rubble2x2 : RubblemakerTile {
    protected Rubble2x2() : base() {
    }
    protected Rubble2x2(string name, bool natural) : base(name, natural) {
    }

    public sealed override FlexibleTileWand Size => FlexibleTileWand.RubblePlacementMedium;

    public override void SafeSetStaticDefaults() {
        Main.tileFrameImportant[Type] = true;
        Main.tileNoAttach[Type] = true;
        Main.tileNoFail[Type] = true;
        Main.tileObsidianKill[Type] = true;

        TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
        TileObjectData.newTile.DrawYOffset = 2;
        if (Natural) {
            TileObjectData.newTile.LavaDeath = false;
        }
        TileObjectData.addTile(Type);
    }
}
