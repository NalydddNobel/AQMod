using System.Diagnostics.CodeAnalysis;

namespace Aequus.Common.Tiles;

public struct TileKey {
    public System.UInt16 TileType;
    public System.Int32 TileStyle;

    public TileKey(System.UInt16 type, System.Int32 style) {
        TileType = type;
        TileStyle = style;
    }

    public TileKey(System.Int32 type, System.Int32 style) : this((System.UInt16)type, style) {
    }

    public TileKey(System.Int32 type) : this((System.UInt16)type, 0) {
    }

    public override System.Int32 GetHashCode() {
        return new { TileType, TileStyle }.GetHashCode();
    }

    public override System.Boolean Equals([NotNullWhen(true)] System.Object obj) {
        if (obj is TileKey tileKey) {
            return TileType == tileKey.TileType && TileStyle == tileKey.TileStyle;
        }
        return base.Equals(obj);
    }

    public static implicit operator TileKey(System.Int32 tileID) {
        return new(tileID, 0);
    }
}