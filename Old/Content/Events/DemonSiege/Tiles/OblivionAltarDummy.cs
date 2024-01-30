using Terraria.ObjectData;

namespace Aequus.Old.Content.Events.DemonSiege.Tiles;

/// <summary>Dummy tile for showing recipes for Gore Nest upgrades. Do not actually use this tile.</summary>
public class OblivionAltarDummy : ModTile {
    public override System.String Texture => AequusTextures.OblivionAltar.Path;

    public override void SetStaticDefaults() {
        Main.tileFrameImportant[Type] = true;
        TileID.Sets.HasOutlines[Type] = true;
        TileID.Sets.DisableSmartCursor[Type] = true;
        TileID.Sets.PreventsTileRemovalIfOnTopOfIt[Type] = true;
        TileID.Sets.PreventsSandfall[Type] = true;

        TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
        TileObjectData.newTile.LavaDeath = false;
        TileObjectData.newTile.AnchorInvalidTiles = new[] { (System.Int32)TileID.MagicalIceBlock, };
        TileObjectData.newTile.CoordinateHeights = new System.Int32[] { 16, 16, 18 };
        TileObjectData.addTile(Type);
        DustType = DustID.Blood;
        AdjTiles = new System.Int32[] { TileID.DemonAltar };
        MineResist = 110;
        AddMapEntry(new Color(175, 15, 15), ExtendLanguage.GetMapEntry<OblivionAltar>());
    }
}