using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Aequus
{
    /// <summary>
    /// Used for darken-ing the sky for flashy visual effects
    /// </summary>
    public class SkyDarkness : ModSystem
    {
        public static float value;
        public static float wantedValue;
        public static float transitionSpeed;

        public override void Load()
        {
            value = 1f;
            On.Terraria.Main.SetBackColor += Main_SetBackColor;
        }

        private static void Main_SetBackColor(On.Terraria.Main.orig_SetBackColor orig, Main.InfoToSetBackColor info, out Color sunColor, out Color moonColor)
        {
            orig(info, out sunColor, out moonColor);

            if (Main.gameMenu)
            {
                value = 1f;
            }
            else if (value != 1f)
            {
                value = Math.Clamp(value, 0.1f, 1f);

                byte a = Main.ColorOfTheSkies.A;
                Main.ColorOfTheSkies *= value;
                Main.ColorOfTheSkies.A = a;

                if (Aequus.GameWorldActive)
                {
                    if (value > 0.9999f)
                    {
                        value = 1f;
                    }
                    value = MathHelper.Lerp(value, wantedValue, transitionSpeed);
                    wantedValue = 1f;
                    transitionSpeed = 0.02f;
                }
            }
        }

        public override void OnWorldLoad()
        {
            value = 1f;
        }

        public override void OnWorldUnload()
        {
            value = 1f;
        }

        public static void DarknessTransition(float to, float speed = 0.05f)
        {
            value -= 0.01f;
            wantedValue = Math.Min(wantedValue, to);
            transitionSpeed = Math.Max(transitionSpeed, speed);
        }
        public static void DarknessSet(float set, float speed = 0.05f)
        {
            value = set;
            transitionSpeed = Math.Max(transitionSpeed, speed);
        }
    }
}