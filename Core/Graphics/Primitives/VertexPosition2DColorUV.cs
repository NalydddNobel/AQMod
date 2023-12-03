using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Aequus.Core.Graphics.Primitives;

public struct VertexPosition2DColorUV : IVertexType {
    public Vector2 Position;

    public Color Color;

    public Vector2 TexCoord;

    private static VertexDeclaration _vertexDeclaration = new VertexDeclaration(new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0), new VertexElement(8, VertexElementFormat.Color, VertexElementUsage.Color, 0), new VertexElement(12, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0));

    public VertexDeclaration VertexDeclaration => _vertexDeclaration;

    public VertexPosition2DColorUV(Vector2 position, Color color, Vector2 texCoord) {
        Position = position;
        Color = color;
        TexCoord = texCoord;
    }
}