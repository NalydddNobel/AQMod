using System;

namespace Aequu2.Core.Utilities;

public static class ExtendCollision {
    public static bool SolidCollision(this Rectangle hitbox) {
        return Collision.SolidCollision(hitbox.TopLeft(), hitbox.Width, hitbox.Height);
    }

    /// <param name="startWorldPosition">The starting scan position.</param>
    /// <param name="worldHeight">How far (in world coordinates) it should scan down. 16 world coordinates = 1 tile coordinate.</param>
    /// <param name="floor">The resulting floor's position.</param>
    /// <param name="acceptTopSurfaces">Whether or not to accept surfaces like Platforms, Tables, Planter Boxes, etc.</param>
    /// <returns>Whether or not a solid floor was found.</returns>
    public static bool GetFloor(Vector2 startWorldPosition, int worldHeight, out Vector2 floor, bool acceptTopSurfaces = true) {
        floor = startWorldPosition;
        int blockHeight = (int)Math.Ceiling(worldHeight / 16f);

        // Swiftly scan through tiles to find a solid one
        bool foundTile = false;
        for (int i = 0; i < blockHeight; i++) {
            if (Collision.SolidTiles(floor, 16, 14, acceptTopSurfaces)) {
                foundTile = true;
                break;
            }

            floor.Y += 16f;
        }

        if (!foundTile) {
            // Return false if no solid tiles were scanned at all.
            return false;
        }

        // Once we have found a solid tile, we now perform more in-depth checks.

        // Snap vertical position to the tile's Y coordinate.
        floor.Y = (int)(floor.Y / 16f) * 16f;

        // Now scan tiles for better collision.
        for (int i = 0; i < 16; i++) {
            //Dust d = Dust.NewDustPerfect(floor, DustID.Torch);
            //d.noGravity = true;
            //d.velocity *= 0.1f;
            if (Collision.IsWorldPointSolid(floor, !acceptTopSurfaces)) {
                // Return true if collision was found
                return true;
            }

            floor.Y += 2f;
        }

        return false;
    }

    public static bool RectangleVsCircle(Vector2 circle, float circleRadius, Rectangle rectangle) {
        return Vector2.Distance(circle, rectangle.Center.ToVector2() + Vector2.Normalize(circle - rectangle.Center.ToVector2()) * rectangle.Size() / 2f) < circleRadius;
    }

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