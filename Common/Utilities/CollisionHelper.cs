using Microsoft.Xna.Framework;
using Terraria;

namespace Aequus.Common.Utilities
{
    public sealed class CollisionHelper
    {
        public static bool IsRectangleCollidingWithCircle(Vector2 circle, float circleRadius, Rectangle rectangle)
        {
            return Vector2.Distance(circle, rectangle.Center.ToVector2() + Vector2.Normalize(circle - rectangle.Center.ToVector2()) * rectangle.Size() / 2f) < circleRadius;
        }
    }
}