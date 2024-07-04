using System;

namespace AequusRemake.Core.Graphics.Primitives;

[Obsolete]
public class UVVertexStrip : VertexStripClone {
    public static readonly UVVertexStrip Instance = new();

    protected float uvMultiplier;
    protected float uvAdd;

    public void PrepareUV(float multiplier = 1f, float add = 0f) {
        uvMultiplier = multiplier;
        uvAdd = add;
    }

    protected override void AddVertex(Vector2 pos, float rot, int indexOnVertexArray, Color color, float width, float textureUV) {
        base.AddVertex(pos, rot, indexOnVertexArray, color, width, textureUV);
    }
}