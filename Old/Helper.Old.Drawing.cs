using System;

namespace Aequus.Core.Utilities;

public static partial class Helper {
    [Obsolete]
    public static void LoopingFrame(this Projectile projectile, int ticksPerFrame) {
        if (++projectile.frameCounter > ticksPerFrame) {
            projectile.frameCounter = 0;
            projectile.frame = (projectile.frame + 1) % Main.projFrames[projectile.type];
        }
    }

    [Obsolete]
    public static float Abs(this float value) {
        return Math.Abs(value);
    }

    [Obsolete]
    public static float CalcProgress(int length, int i) {
        return 1f - 1f / length * i;
    }

    [Obsolete]
    public static float[] GenerateRotationArr(Vector2[] oldPos) {
        float[] rotations = new float[oldPos.Length];

        for (int i = 0; i < oldPos.Length; i++) {
            rotations[i] = GetRotationVector(oldPos, i).ToRotation();
        }

        return rotations;
    }

    [Obsolete]
    public static Vector2 GetRotationVector(Vector2[] oldPos, int index) {
        if (oldPos.Length == 1) {
            return Vector2.UnitY;
        }

        if (index == 0) {
            return Utils.SafeNormalize(oldPos[1] - oldPos[0], Vector2.UnitY);
        }

        if (index == oldPos.Length - 1) {
            return Utils.SafeNormalize(oldPos[index] - oldPos[index - 1], Vector2.UnitY);
        }

        return Utils.SafeNormalize(oldPos[index + 1] - oldPos[index - 1], Vector2.UnitY);
    }

    [Obsolete]
    public static void DrawFramedChain(Texture2D chain, Rectangle frame, Vector2 currentPosition, Vector2 endPosition, Vector2 screenPos, Func<Vector2, Color> getLighting = null) {
        getLighting ??= ExtendLight.Get;
        int height = frame.Height - 2;
        Vector2 velocity = endPosition - currentPosition;
        int length = (int)(velocity.Length() / height);
        velocity.Normalize();
        velocity *= height;
        float rotation = velocity.ToRotation() + MathHelper.PiOver2;
        var origin = new Vector2(frame.Width / 2f, frame.Height / 2f);
        for (int i = 0; i < length + 1; i++) {
            var position = currentPosition + velocity * i;
            Main.EntitySpriteDraw(chain, position - screenPos, frame, getLighting(position), rotation, origin, 1f, SpriteEffects.None, 0);
        }
    }

    [Obsolete]
    public static void DrawChain(Texture2D chain, Vector2 currentPosition, Vector2 endPosition, Vector2 screenPos, Func<Vector2, Color> getLighting = null) {
        DrawFramedChain(chain, chain.Bounds, currentPosition, endPosition, screenPos, getLighting);
    }
}
