using System.Diagnostics.CodeAnalysis;

namespace AequusRemake.Core.Entities.Tiles;
public struct TileKey {
    public ushort TileType;
    public int TileStyle;

    public TileKey(ushort type, int style) {
        TileType = type;
        TileStyle = style;
    }

    public TileKey(int type, int style) : this((ushort)type, style) {
    }

    public TileKey(int type) : this((ushort)type, 0) {
    }

    public override int GetHashCode() {
        return new { TileType, TileStyle }.GetHashCode();
    }

    public override bool Equals([NotNullWhen(true)] object obj) {
        if (obj is TileKey tileKey) {
            return TileType == tileKey.TileType && TileStyle == tileKey.TileStyle;
        }
        return base.Equals(obj);
    }

    public static implicit operator TileKey(int tileID) {
        return new(tileID, 0);
    }
}