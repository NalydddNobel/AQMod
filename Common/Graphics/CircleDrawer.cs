using Microsoft.Xna.Framework;
using System;
using Terraria;

namespace Aequus.Common.Graphics;
public class CircleDrawer {
    public static void GetVertices(Vector2[] frontCoordinates, float[] frontRotations, Vector2[] backCoordinates, float[] backRotations, Vector2 radius, float pitch = 0f, float roll = 0f) {
        var multiplier = new Vector2(1f, 1f - Math.Abs(pitch) % MathHelper.Pi / MathHelper.Pi);
        float rotationMultiplier = Math.Sign(multiplier.Y);
        for (int i = 0; i < frontCoordinates.Length; i++) {
            float rotation = i * MathHelper.Pi / (frontCoordinates.Length - 1);
            var offset = (new Vector2(radius.Length(), 0f).RotatedBy(rotation) * multiplier).RotatedBy(roll);
            frontCoordinates[i] = offset * radius;
            frontRotations[i] = rotation * rotationMultiplier - MathHelper.PiOver2 + roll;

            backCoordinates[i] = offset.RotatedBy(MathHelper.Pi);
            backRotations[i] = rotation * rotationMultiplier + MathHelper.PiOver2 + roll;
        }
    }
}