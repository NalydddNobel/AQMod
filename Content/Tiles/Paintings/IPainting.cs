namespace Aequus.Content.Tiles.Paintings;

public interface IPainting {
    Mod Mod { get; }
    int Width { get; }
    int Height { get; }
    ushort TileType { get; }
    int ItemType { get; }
}
