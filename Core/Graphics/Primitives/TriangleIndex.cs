using System;

namespace Aequus.Core.Graphics.Primitives;
public readonly record struct TriangleIndex(short I1, short I2, short I3) {
    public short this[int index] {
        get {
            return index switch {
                0 => I1,
                1 => I2,
                2 => I3,
                _ => throw new IndexOutOfRangeException()
            };
        }
    }

    public static implicit operator TriangleIndex(ValueTuple<short, short, short> tuple) {
        return new(tuple.Item1, tuple.Item2, tuple.Item3);
    }
}