using System.Collections.Generic;
using System.Reflection;
using Terraria.GameContent.Drawing;

namespace Aequus.Common.Drawing;

internal class TileDrawHooks : LoadedType {
    public static TileDrawHooks Instance => ModContent.GetInstance<TileDrawHooks>();

    public readonly Dictionary<int, HangingTileInfo> HangingTile = [];

    private static void On_TileDrawing_DrawMultiTileVinesInWind(On_TileDrawing.orig_DrawMultiTileVinesInWind orig, TileDrawing self, Vector2 screenPosition, Vector2 offSet, int topLeftX, int topLeftY, int sizeX, int sizeY) {
        if (Instance.HangingTile.TryGetValue(Main.tile[topLeftX, topLeftY].TileType, out var value)) {
            sizeX = value.X ?? sizeX;
            sizeY = value.Y ?? sizeY;
        }

        orig(self, screenPosition, offSet, topLeftX, topLeftY, sizeX, sizeY);
    }

    #region Vanilla Draw points
    private readonly FieldInfo? _addSpecialPointSpecialPositions = typeof(TileDrawing).GetField("_specialPositions", BindingFlags.NonPublic | BindingFlags.Instance);
    private readonly FieldInfo? _addSpecialPointSpecialsCount = typeof(TileDrawing).GetField("_specialsCount", BindingFlags.NonPublic | BindingFlags.Instance);
    private Point[][]? _specialPositions;
    private int[]? _specialsCount;

    void LoadTileDrawingReflection(TileDrawing renderer) {
        _specialPositions = (Point[][]?)_addSpecialPointSpecialPositions?.GetValue(renderer);
        _specialsCount = (int[]?)_addSpecialPointSpecialsCount?.GetValue(renderer);
    }

    public void AddSpecialPoint(int x, int y, int type) {
        if (_specialPositions == null || _specialsCount == null) {
            return;
        }

        _specialPositions[type][_specialsCount[type]++] = new Point(x, y);
    }
    #endregion

    protected override void Load() {
        On_TileDrawing.DrawMultiTileVinesInWind += On_TileDrawing_DrawMultiTileVinesInWind;
        LoadTileDrawingReflection(Main.instance.TilesRenderer);
    }
}

public readonly record struct HangingTileInfo(int? X, int? Y) {
    public static implicit operator HangingTileInfo(int value) {
        return new(null, value);
    }
}
