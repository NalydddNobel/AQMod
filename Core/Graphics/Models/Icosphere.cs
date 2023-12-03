using Aequus.Core.Graphics.Primitives;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Aequus.Core.Graphics.Models;

public class Icosphere : Icosahedron {
    private readonly int _subDivideCount;

    public Icosphere(int subDivideCount) {
        _subDivideCount = subDivideCount;
    }


    protected override void GenerateInner() {
        AddVertices(BaseVertices);
        AddTriangles(PerformSubdivides());
    }

    public TriangleIndex[] PerformSubdivides() {
        var triangles = new TriangleIndex[BaseIndices.Length];
        Array.Copy(BaseIndices, triangles, triangles.Length);
        var midPointCache = new Dictionary<short, short>();
        for (int d = 0; d < _subDivideCount; d++) {
            var newTriangles = new List<TriangleIndex>();
            foreach (var t in triangles) {
                var a = t[0];
                var b = t[1];
                var c = t[2];

                short ab = GetMidPointIndex(midPointCache, a, b);
                short bc = GetMidPointIndex(midPointCache, b, c);
                short ca = GetMidPointIndex(midPointCache, c, a);

                newTriangles.Add(new(a, ab, ca));
                newTriangles.Add(new(b, bc, ab));
                newTriangles.Add(new(c, ca, bc));
                newTriangles.Add(new(ab, bc, ca));
            }
            triangles = newTriangles.ToArray();
        }
        return triangles;
    }

    public short GetMidPointIndex(Dictionary<short, short> cache, short a, short b) {
        short smallerIndex = Math.Min(a, b);
        short greaterIndex = Math.Max(a, b);
        short key = (short)((smallerIndex << 16) + greaterIndex);

        if (cache.TryGetValue(key, out short ret)) {
            return ret;
        }

        var p1 = Vertices[a];
        var p2 = Vertices[b];
        var middle = (p1.Position + p2.Position) / 2f;

        ret = (short)Vertices.Count;

        AddVertex(Vector3.Normalize(middle));

        cache.Add(key, ret);
        return ret;
    }
}