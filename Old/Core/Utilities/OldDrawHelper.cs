using System;
using Terraria.GameContent;

namespace Aequus.Old.Core.Utilities;

public static class OldDrawHelper {
    public static void GetDrawInfo(this NPC npc, out Texture2D texture, out Vector2 offset, out Rectangle frame, out Vector2 origin, out int trailLength) {
        texture = TextureAssets.Npc[npc.type].Value;
        offset = npc.Size / 2f;
        frame = npc.frame;
        origin = frame.Size() / 2f;
        trailLength = NPCSets.TrailCacheLength[npc.type];
    }

    public static float[] GenerateRotationArr(Vector2[] oldPos) {
        float[] rotations = new float[oldPos.Length];

        for (int i = 0; i < oldPos.Length; i++) {
            rotations[i] = GetRotationVector(oldPos, i).ToRotation();
        }

        return rotations;
    }

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
    public static void DrawChain(Texture2D chain, Vector2 currentPosition, Vector2 endPosition, Vector2 screenPos, Func<Vector2, Color> getLighting = null) {
        DrawFramedChain(chain, chain.Bounds, currentPosition, endPosition, screenPos, getLighting);
    }
}
