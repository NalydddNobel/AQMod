using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Aequus.Tiles.CraftingStations;

/// <summary>
/// Dummy tile for showing recipes for Gore Nest upgrades. Do not actually use this tile.
/// </summary>
public class GoreNestDummy : ModTile {
    public override string Texture => AequusTextures.GoreNestTile.FullPath;

    public override void SetStaticDefaults() {
        Main.tileFrameImportant[Type] = true;
        TileID.Sets.HasOutlines[Type] = true;
        TileID.Sets.DisableSmartCursor[Type] = true;
        TileID.Sets.PreventsTileRemovalIfOnTopOfIt[Type] = true;
        TileID.Sets.PreventsSandfall[Type] = true;

        TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
        TileObjectData.newTile.LavaDeath = false;
        TileObjectData.newTile.AnchorInvalidTiles = new[] { (int)TileID.MagicalIceBlock, };
        TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 18 };
        TileObjectData.addTile(Type);
        DustType = DustID.Blood;
        AdjTiles = new int[] { TileID.DemonAltar };
        MineResist = 110;
        AddMapEntry(new(175, 15, 15), TextHelper.GetItemName<GoreNest>());
    }
}