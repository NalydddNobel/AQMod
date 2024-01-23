namespace Aequus.Core.Utilities;

public static class ExtendCollision {
    public static bool LineHitbox(Vector2 center, Rectangle targetHitbox, float rotation, float length, float size, float startLength = 0f) {
        return LineHitbox(center, targetHitbox, rotation.ToRotationVector2(), length, size, startLength);
    }
    public static bool LineHitbox(Vector2 center, Rectangle targetHitbox, Vector2 normal, float length, float size, float startLength = 0f) {
        return LineHitbox(center + normal * startLength, center + normal * startLength + normal * length, targetHitbox, size);
    }
    public static bool LineHitbox(Vector2 from, Vector2 to, Rectangle targetHitbox, float size) {
        float _ = float.NaN;
        return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), from, to, size, ref _);
    }
}