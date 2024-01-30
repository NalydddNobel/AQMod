using System;

namespace Aequus.Core.Graphics.Primitives;

[Obsolete]
public class UVVertexStrip : VertexStripClone {
    public static readonly UVVertexStrip Instance = new();

    protected Single uvMultiplier;
    protected Single uvAdd;

    public void PrepareUV(Single multiplier = 1f, Single add = 0f) {
        uvMultiplier = multiplier;
        uvAdd = add;
    }

    protected override void AddVertex(Vector2 pos, Single rot, Int32 indexOnVertexArray, Color color, Single width, Single textureUV) {
        base.AddVertex(pos, rot, indexOnVertexArray, color, width, textureUV * 10f % 1f);
    }
}