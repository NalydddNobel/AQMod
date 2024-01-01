using Microsoft.Xna.Framework;

namespace Aequus.Core.Utilities;

public static class CollisionHelper {
    public static bool DeathrayHitbox(Vector2 center, Rectangle targetHitbox, float rotation, float length, float size, float startLength = 0f) {
        return DeathrayHitbox(center, targetHitbox, rotation.ToRotationVector2(), length, size, startLength);
    }
    public static bool DeathrayHitbox(Vector2 center, Rectangle targetHitbox, Vector2 normal, float length, float size, float startLength = 0f) {
        return DeathrayHitbox(center + normal * startLength, center + normal * startLength + normal * length, targetHitbox, size);
    }
    public static bool DeathrayHitbox(Vector2 from, Vector2 to, Rectangle targetHitbox, float size) {
        float _ = float.NaN;
        return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), from, to, size, ref _);
    }
}