﻿using System;

namespace Aequus.Old.Core.Utilities;

public static class OldDrawHelper {
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
