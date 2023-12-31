using System;

namespace Aequus.Common.Tiles;

public enum TileAnchorDirection : byte {
    None = 0,
    Wall,
    Bottom,
    Top,
    Right,
    Left
}

[Flags]
public enum TileAnchorFlags : byte {
    None = 0,
    Wall = 1 << 0,
    Bottom = 1 << 1,
    Top = 1 << 2,
    Right = 1 << 3,
    Left = 1 << 4
}