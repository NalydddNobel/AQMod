using System;
using Terraria.Utilities;

namespace Aequus.Common.Utilities
{
    public class MiniRandom
    {
        private int seedCache;
        private byte[] samples;
        private int sampleIndex;

        public MiniRandom(int seed, int capacity = byte.MaxValue + 1)
        {
            seedCache = seed;
            samples = new byte[byte.MaxValue + 1];
            new UnifiedRandom(seed).NextBytes(samples);
        }
        public MiniRandom(string seed, int capacity = byte.MaxValue + 1) : this(seed.GetHashCode(), capacity)
        {
        }

        public byte Rand()
        {
            byte value = samples[sampleIndex];
            IncRand(1);
            return value;
        }
        public bool RandChance(int chance)
        {
            return AequusHelpers.FromByte(Rand(), chance) < 1f;
        }
        public float Rand(float max)
        {
            return AequusHelpers.FromByte(Rand(), max);
        }
        public float Rand(float min, float max)
        {
            return AequusHelpers.FromByte(Rand(), min, max);
        }
        public void IncRand(int amount)
        {
            sampleIndex = (sampleIndex + amount) % samples.Length;
        }
        public int SetRand(int set)
        {
            int oldValue = sampleIndex;
            sampleIndex = 0;
            IncRand(Math.Abs(set));
            return oldValue;
        }
    }
}