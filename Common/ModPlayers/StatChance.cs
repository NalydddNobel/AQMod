using Microsoft.Xna.Framework;
using System;
using Terraria;

namespace Aequus.Common.ModPlayers
{
    public struct StatChance
    {
        private float value;

        public float Chance => value;
        public float ChanceClamped => MathHelper.Clamp(value, 0f, 1f);
        public int RollChance => Math.Max((int)(ChanceClamped * 100), 1);
        public bool AlwaysRollsFalse => value >= 1;
        public bool AlwaysRollsTrue => value <= 0;

        public void Clear()
        {
            value = 1f;
        }

        public void AdjustChance(float percent)
        {
            value *= percent;
        }

        public bool Roll()
        {
            return AlwaysRollsFalse ? false : Main.rand.NextBool(RollChance);
        }
        public bool RollLuck(Player player)
        {
            return player.RollLuck(100) < 100 - RollChance;
        }

        public static StatChance operator *(StatChance chance, float value)
        {
            return chance with { value = chance.value * value };
        }
    }
}