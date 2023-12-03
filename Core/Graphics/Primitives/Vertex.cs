using Microsoft.Xna.Framework;
using System;

namespace Aequus.Core.Graphics.Primitives;

public readonly record struct Vertex(float X, float Y, float Z) {
    public static implicit operator Vector3(Vertex v) {
        return new(v.X, v.Y, v.Z);
    }
    public static implicit operator Vertex(ValueTuple<float, float, float> tuple) {
        return new(tuple.Item1, tuple.Item2, tuple.Item3);
    }
}