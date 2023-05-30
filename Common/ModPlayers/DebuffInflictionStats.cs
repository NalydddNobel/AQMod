using System.Collections.Generic;
using Terraria;

namespace Aequus.Common.ModPlayers {
    public struct DebuffInflictionStats
    {
        public float OverallTimeMultiplier;

        private readonly Dictionary<int, Ref<float>> PerBuffTimeMultiplier;

        public DebuffInflictionStats(int useless)
        {
            OverallTimeMultiplier = 1f;
            PerBuffTimeMultiplier = new Dictionary<int, Ref<float>>();
        }

        public void Clear(Player player)
        {
            OverallTimeMultiplier = 1f;
            PerBuffTimeMultiplier?.Clear();
        }

        public float GetBuffMultipler(Player player, int type)
        {
            float result = OverallTimeMultiplier;
            if (PerBuffTimeMultiplier.TryGetValue(type, out var val))
            {
                result += val.Value;
            }
            return result;
        }

        public ref float GetBuffMultiplier(Player player, int type)
        {
            if (PerBuffTimeMultiplier.TryGetValue(type, out var val))
            {
                return ref val.Value;
            }
            var v = new Ref<float>(1f);
            PerBuffTimeMultiplier.Add(type, val);
            return ref val.Value;
        }
    }
}