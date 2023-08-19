using Microsoft.Xna.Framework;

namespace Aequus.Common.Graphics.Primitives;

public class ChainVertexStrip : BaseVertexStrip {
    protected override void GetVerticalCoordinates(Vector2 frontWidth, Vector2 backWidth, ref float UVFront, ref float UVBack) {
        UVFront = 1f;
        UVBack = 0f;
    }

    protected override void GetHorizontalCoordinates(ref float UVFront, ref float UVBack) {
        UVFront = 1f;
        UVBack = 0f;
    }
}