using Terraria;
using Terraria.Utilities;

namespace AQMod.Common.Utilities
{
    public struct ValueRange
    {
        public static ValueRange SingleItem => new ValueRange(1);

        private int _min;
        public int MinimumAmount => _min;

        private int _max;
        public int MaximumAmount => _max;

        public bool HasAnyRange => _min != _max;

        public ValueRange(int chance)
        {
            _min = chance;
            _max = chance;
        }

        public ValueRange(int min, int max)
        {
            _min = min;
            _max = max;
        }

        public int GetAValue()
        {
            return GetAValue(Main.rand);
        }

        public int GetAValue(UnifiedRandom rand)
        {
            return HasAnyRange ? rand.NextVRand(_min, _max) : _min;
        }
    }
}