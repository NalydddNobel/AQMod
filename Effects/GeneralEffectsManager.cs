using AQMod.Effects.ScreenEffects;
using System.Collections.Generic;
using Terraria;
using Terraria.Utilities;

namespace AQMod.Effects
{
    public static class GeneralEffectsManager
    {
        private static UnifiedRandom _random;
        private static readonly int RandomSeed = "Split".GetHashCode();
        public static Dictionary<string, ScreenShakeFX> ScreenShakes;

        private static byte _randBytesIndex;
        private static byte[] _randBytes;

        internal static void InternalSetup()
        {
            _random = new UnifiedRandom(RandomSeed);

            _randBytes = new byte[byte.MaxValue + 1];
            _random.NextBytes(_randBytes);
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
    }
}
