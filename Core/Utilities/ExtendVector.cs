using System.Collections.Generic;

namespace Aequus.Core.Utilities;

public static class ExtendVector {
    public static IEnumerable<Vector2> Circular(int amount) {
        float rotation = amount / MathHelper.TwoPi + float.Epsilon;
        for (float r = 0f; r < MathHelper.TwoPi; r += rotation) {
            yield return r.ToRotationVector2();
        }
    }
}
