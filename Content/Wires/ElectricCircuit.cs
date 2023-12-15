using Microsoft.Xna.Framework;

namespace Aequus.Content.Wires;

public readonly record struct ElectricCircuit(Point Position, byte Direction, byte SplitCount = 0, byte TurnCounts = 0) {
    public const byte Right = 0;
    public const byte Left = 1;
    public const byte Up = 2;
    public const byte Down = 3;
    public const byte Dead = 4;
    public const byte DirectionCount = 4;

    public Point PosForward => Position + DirectionValues[Direction];
    public Point PosLeft => Position + DirectionValues[DirLeft];
    public Point PosRight => Position + DirectionValues[DirRight];

    public byte DirLeft => LeftTransform[Direction];
    public byte DirRight => RightTransform[Direction];

    public static readonly Point[] DirectionValues = new Point[] {
        new(1, 0),
        new(-1, 0),
        new(0, 1),
        new(0, -1),
    };

    public static readonly byte[] LeftTransform = new byte[] {
        Down,
        Up,
        Left,
        Right
    };

    public static readonly byte[] RightTransform = new byte[] {
        Up,
        Down,
        Right,
        Left
    };
}