using Microsoft.Xna.Framework;

namespace Aequus.Content.Wires;

public readonly record struct ElectricCircuit(Point Position, byte Direction, byte SplitCount = 0) {
    public const byte Right = 0;
    public const byte Left = 1;
    public const byte Up = 2;
    public const byte Down = 3;
    public const byte Dead = 4;
    public const byte DirectionCount = 4;

    public static readonly Point[] DirectionIds = new Point[] {
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