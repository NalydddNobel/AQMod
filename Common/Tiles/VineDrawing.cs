using Aequus.Core.Graphics.Tiles;
using System.Collections.Generic;
using Terraria.GameContent.Drawing;

namespace Aequus.Common.Tiles;

public class VineDrawing : ILoadable {
    public static readonly Dictionary<System.Int32, System.Int32> VineLength = new();

    public static void DrawVine(System.Int32 i, System.Int32 j) {
        SpecialTileRenderer.AddVanillaSpecialPoint(i, j, 5);
    }

    private static void On_TileDrawing_DrawMultiTileVinesInWind(On_TileDrawing.orig_DrawMultiTileVinesInWind orig, TileDrawing self, Vector2 screenPosition, Vector2 offSet, System.Int32 topLeftX, System.Int32 topLeftY, System.Int32 sizeX, System.Int32 sizeY) {
        // Turn these into IL edits later?

        if (VineLength.TryGetValue(Main.tile[topLeftX, topLeftY].TileType, out System.Int32 value)) {
            sizeY = value;
        }

        // Strange drawing hack used by Starite in a Bottle

        //var t = Main.tile[topLeftX, topLeftY];
        //var type = t.TileType;
        //int renderConversion = TileSets.TileRenderConversion[t.TileType];
        //if (renderConversion > 0) {
        //    TileChange = t.TileType;
        //    for (int i = topLeftX; i < topLeftX + sizeX; i++) {
        //        for (int j = topLeftY; j < topLeftY + sizeY; j++) {
        //            Main.tile[i, j].TileType = (ushort)renderConversion;
        //        }
        //    }
        //}

        orig(self, screenPosition, offSet, topLeftX, topLeftY, sizeX, sizeY);

        //if (TileChange > 0) {
        //    for (int i = topLeftX; i < topLeftX + sizeX; i++) {
        //        for (int j = topLeftY; j < topLeftY + sizeY; j++) {
        //            Main.tile[i, j].TileType = (ushort)TileChange;
        //        }
        //    }
        //    TileChange = 0;
        //}
    }

    public void Load(Mod mod) {
        On_TileDrawing.DrawMultiTileVinesInWind += On_TileDrawing_DrawMultiTileVinesInWind;
    }

    public void Unload() { }
}