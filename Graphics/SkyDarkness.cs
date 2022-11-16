using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Graphics
{
    /// <summary>
    /// Used for darken-ing the sky for flashy visual effects
    /// </summary>
    public class SkyDarkness : ModSystem
    {
        public static float Value { get; private set; }
        public static float WantedValue { get; private set; }
        public static float TransitionSpeed { get; private set; }

        public override void Load()
        {
            Value = 1f;
            On.Terraria.Main.SetBackColor += Hook_DarkenBackground;
        }

        private static void Hook_DarkenBackground(On.Terraria.Main.orig_SetBackColor orig, Main.InfoToSetBackColor info, out Color sunColor, out Color moonColor)
        {
            orig(info, out sunColor, out moonColor);

            if (Main.gameMenu)
            {
                Value = 1f;
            }
            else if (Value != 1f)
            {
                Value = Math.Clamp(Value, 0.1f, 1f);

                byte a = Main.ColorOfTheSkies.A;
                Main.ColorOfTheSkies *= Value;
                Main.ColorOfTheSkies.A = a;

                if (Aequus.GameWorldActive)
                {
                    if (Value > 0.9999f)
                    {
                        Value = 1f;
                    }
                    Value = MathHelper.Lerp(Value, WantedValue, TransitionSpeed);
                    WantedValue = 1f;
                    TransitionSpeed = 0.02f;
                }
            }
        }

        public override void OnWorldLoad()
        {
            Value = 1f;
        }

        public override void OnWorldUnload()
        {
            Value = 1f;
        }

        public static void DarkenSky(float to, float speed = 0.05f)
        {
            Value -= 0.01f;
            WantedValue = Math.Min(WantedValue, to);
            TransitionSpeed = Math.Max(TransitionSpeed, speed);
        }
    }
}