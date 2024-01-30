namespace Aequus.Core.Utilities;

public static class ExtendCollision {
    public static System.Boolean LineHitbox(Vector2 center, Rectangle targetHitbox, System.Single rotation, System.Single length, System.Single size, System.Single startLength = 0f) {
        return LineHitbox(center, targetHitbox, rotation.ToRotationVector2(), length, size, startLength);
    }
    public static System.Boolean LineHitbox(Vector2 center, Rectangle targetHitbox, Vector2 normal, System.Single length, System.Single size, System.Single startLength = 0f) {
        return LineHitbox(center + normal * startLength, center + normal * startLength + normal * length, targetHitbox, size);
    }
    public static System.Boolean LineHitbox(Vector2 from, Vector2 to, Rectangle targetHitbox, System.Single size) {
        System.Single _ = System.Single.NaN;
        return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), from, to, size, ref _);
    }
}