using Aequus;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Aequus
{
    public class ScreenShake : ModSystem
    {
        public static float Intensity;
        public static float MultiplyPerTick;

        private static Vector2 screenOffset;

        public static Vector2 ScreenOffset => screenOffset;

        public override void PreUpdatePlayers()
        {
            if (Intensity > 1f)
            {
                screenOffset = (new Vector2(Main.rand.NextFloat(-Intensity, Intensity), Main.rand.NextFloat(-Intensity, Intensity)) * 0.5f).Floor();
                Intensity *= MultiplyPerTick;
            }
            else
            {
                ClearShake();
            }
        }

        public static void SetShake(float intensity, float multiplier = 0.9f, Vector2 where = default(Vector2))
        {
            if (where != default(Vector2))
            {
                float distance = Vector2.Distance(where, Main.screenPosition + new Vector2(Main.screenWidth / 2f, Main.screenHeight / 2f)) / 1200f;
                if (float.IsNaN(distance) || distance >= 1f)
                    return;
                Intensity *= 1f - distance;
            }

            Intensity = Math.Max(Intensity, intensity);
            MultiplyPerTick = Math.Min(MultiplyPerTick, multiplier);
        }
        public static void ClearShake()
        {
            Intensity = 0f;
            MultiplyPerTick = 0.9f;
            screenOffset = new Vector2();
        }
    }
}