using Aequus.Core.Graphics.Primitives;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xna.Framework;

namespace Aequus.Core.Graphics.Models;

public class Icosahedron : GenMesh {
    private const float X = 0.525731112119133606f;
    private const float Z = 0.850650808352039932f;
    private const float N = 0f;

    public static readonly Vertex[] BaseVertices =
    {
        (-X,N,Z),  (X,N,Z),    (-X,N,-Z),  (X,N,-Z),
        (N,Z,X),   (N,Z,-X),   (N,-Z,X),   (N,-Z,-X),
        (Z,X,N),   (-Z,X, N),  (Z,-X,N),   (-Z,-X, N)
    };

    public static readonly TriangleIndex[] BaseIndices =
    {
        (0,4,1),   (0,9,4),   (9,5,4),   (4,5,8),   (4,8,1),
        (8,10,1),  (8,3,10),  (5,3,8),   (5,2,3),   (2,7,3),
        (7,10,3),  (7,6,10),  (7,11,6),  (11,0,6),  (0,1,6),
        (6,1,10),  (9,0,11),  (9,11,2),  (9,2,5),   (7,2,11)
    };

    protected override void GenerateInner() {
        AddVertices(BaseVertices);
        AddTriangles(BaseIndices);
    }
}