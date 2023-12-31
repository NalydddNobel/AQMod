namespace Aequus.Common.Tiles;

public record struct TileKey(ushort TileType, ushort TileStyle) {
    public TileKey(ushort TileType, int TileStyle) : this(TileType, (ushort)TileStyle) { }
    public TileKey(int TileType, int TileStyle) : this((ushort)TileType, (ushort)TileStyle) { }
    public TileKey(int TileType) : this((ushort)TileType, (ushort)0) { }

    public override int GetHashCode() => new { TileType, TileStyle }.GetHashCode();

    public static implicit operator TileKey(int TileType) => new((ushort)TileType, (ushort)0);
}