namespace Aequus;

public record struct TileKey(ushort Type, int Style) {
    public TileKey(int type, int style) : this((ushort)type, style) { }

    public TileKey(int type) : this((ushort)type, 0) { }

    public static implicit operator TileKey(int type) {
        return new(type, 0);
    }
    public static implicit operator TileKey((int type, int style) valuePair) {
        return new(valuePair.type, valuePair.style);
    }
}