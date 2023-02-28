using System;
using Terraria.Utilities;

namespace Aequus.Common.Utilities
{
    [Obsolete("Use Terraria.Utilities.FastRandom instead.")]
    public class CachedRandom
    {
        public int seedCache;
        public byte[] samples;
        public int sampleIndex;

        public CachedRandom(int seed, int capacity = byte.MaxValue + 1)
        {
            seedCache = seed;
            samples = new byte[byte.MaxValue + 1];
            new UnifiedRandom(seed).NextBytes(samples);
        }
        public CachedRandom(string seed, int capacity = byte.MaxValue + 1) : this(seed.GetHashCode(), capacity)
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
            return Helper.FromByte(Rand(), chance) < 1f;
        }
        public float Rand(float max)
        {
            return Math.Min(Helper.FromByte(Rand(), max), max);
        }
        public float Rand(float min, float max)
        {
            return Helper.FromByte(Rand(), min, max);
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