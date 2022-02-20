using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Utilities;

namespace AQMod.Effects
{
    public static class FX
    {
        public class ScreenShake
        {
            public virtual bool Active => intensity > 0f;
            public float intensity;
            public float intensityCap;
            public float IntensityCap
            {
                get => intensityCap;

                set
                {
                    if (value > intensityCap)
                    {
                        intensityCap = value;
                    }
                }
            }
            public readonly float baseDecrement;
            public float decrement;
            public float a;

            private Func<ScreenShake, Vector2> getOffset;
            private Action<ScreenShake> update;
            public ScreenShake(Func<ScreenShake, Vector2> getOffset)
            {
                this.getOffset = getOffset;
                update = BaseUpdate;
                decrement = baseDecrement = 0.05f;
            }

            public ScreenShake(Func<ScreenShake, Vector2> getOffset, Action<ScreenShake> update)
            {
                this.getOffset = getOffset;
                this.update = update;
                decrement = baseDecrement = 0.05f;
            }

            private static void BaseUpdate(ScreenShake screenShake)
            {
                if (screenShake.intensity < 0f)
                {
                    screenShake.intensity = 0f;
                    screenShake.intensityCap = 0f;
                    screenShake.decrement = screenShake.baseDecrement;
                }
                else if (screenShake.intensity != 0f)
                {
                    if (screenShake.decrement < 0.05f)
                    {
                        screenShake.decrement = 0.05f;
                    }
                    screenShake.intensity -= screenShake.decrement;
                }
            }

            public virtual void Update()
            {
                update(this);
            }

            public virtual Vector2 GetOffset()
            {
                return getOffset(this);
            }
        }

        private static UnifiedRandom _random;
        private static readonly int RandomSeed = "Split".GetHashCode();

        public static Dictionary<string, ScreenShake> ScreenShakes { get; private set; }

        public static Vector2 CameraFocus = Vector2.Zero;
        public static bool cameraFocus = false;
        public static int cameraFocusNPC = -1;
        public static int cameraFocusResetDelay = -1;
        public static float cameraFocusLerp = 0f;

        public static Vector2 flashLocation = Vector2.Zero;
        public static float flashBrightness = 0f;
        public static float flashBrightnessDecrement = 0f;

        private static byte _randBytesIndex = 0;
        private static byte[] _randBytes;

        internal static void InternalSetup()
        {
            _random = new UnifiedRandom(RandomSeed);

            _randBytes = new byte[byte.MaxValue + 1];
            _random.NextBytes(_randBytes);

            ScreenShakes = new Dictionary<string, ScreenShake>()
            {
                ["ScreenShake"] = new ScreenShake((s) => new Vector2(Rand(-s.intensity, s.intensity), Rand(-s.intensity, s.intensity))),
            };
        }
        internal static void Unload()
        {
            _random = null;
            _randBytes = null;
            _randBytesIndex = 0;
            ScreenShakes = null;
        }

        internal static void InternalInitalize()
        {
            _random = new UnifiedRandom(RandomSeed + (int)Main.GameUpdateCount);
        }

        public static byte Rand()
        {
            byte value = _randBytes[_randBytesIndex];
            _randBytesIndex++;
            return value;
        }
        public static bool RandChance(int chance)
        {
            return AQUtils.FromByte(Rand(), chance) < 1f;
        }
        public static float Rand(float max)
        {
            return AQUtils.FromByte(Rand(), max);
        }
        public static float Rand(float min, float max)
        {
            return AQUtils.FromByte(Rand(), min, max);
        }
        public static void IncRand(int amount)
        {
            int newIndex = _randBytesIndex + amount;
            _randBytesIndex = (byte)(newIndex % byte.MaxValue);
        }
        public static void SetRand(int set)
        {
            SetRand((byte)(set % byte.MaxValue));
        }
        public static void SetRand(byte set)
        {
            _randBytesIndex = set;
        }

        public static void TempSetRand(int set, out int reset)
        {
            reset = _randBytesIndex;
            SetRand((byte)(set % byte.MaxValue));
        }

        public static void SetShake(float intensity)
        {
            var s = ScreenShakes["ScreenShake"];
            if (s.intensity < intensity)
            {
                s.intensity = intensity;
                if (s.intensityCap < intensity)
                {
                    s.intensityCap = intensity;
                }
            }
        }

        public static void SetShake(float intensity, float time)
        {
            var s = ScreenShakes["ScreenShake"];
            if (s.intensity < intensity)
            {
                s.intensity = intensity;
                if (s.intensityCap < intensity)
                {
                    s.intensityCap = intensity;
                }
                s.decrement = s.intensity / time;
            }
        }

        public static void AddShake(float intensity, float max)
        {
            var s = ScreenShakes["ScreenShake"];
            if (s.intensity < intensity)
            {
                if (s.intensityCap < max)
                {
                    s.intensityCap = max;
                }
                s.intensity += intensity;
                if (s.intensity > s.intensityCap)
                {
                    s.intensity = s.intensityCap;
                }
            }
        }

        public static void AddShake(float intensity, float max, float time)
        {
            var s = ScreenShakes["ScreenShake"];
            if (s.intensity < intensity)
            {
                if (s.intensityCap < max)
                {
                    s.intensityCap = max;
                }
                s.intensity += intensity;
                if (s.intensity > s.intensityCap)
                {
                    s.intensity = s.intensityCap;
                }
                s.decrement = s.intensity / time;
            }
        }

        public static void SetFlash(Vector2 location, float brightness)
        {
            flashLocation = location;
            flashBrightness = brightness / 18f;
            flashBrightnessDecrement = brightness * 0.2f;
        }

        public static void SetFlash(Vector2 location, float brightness, float time)
        {
            flashLocation = location;
            flashBrightness = brightness;
            flashBrightnessDecrement = flashBrightness * (1f / time);
        }

        internal static void Update()
        {
            foreach (var s in ScreenShakes)
            {
                s.Value.Update();
            }
        }

        public static Vector2 FlippedScreenCheck(Vector2 position)
        {
            return Main.player[Main.myPlayer].gravDir == -1 ? new Vector2(position.X, -position.Y + Main.screenHeight) : new Vector2(position.X, position.Y);
        }
    }
}