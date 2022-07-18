using Aequus.Items.Weapons.Summon.Candles;
using System;
using Terraria;

namespace Aequus.Items.Prefixes.SoulCandles
{
    public class DiabolicPrefix : SoulCandlePrefix
    {
        public override void Apply(Item item)
        {
            if (item.ModItem is SoulCandle candle)
            {
                candle.ClearPrefix();
                candle.soulLimit = Math.Max((int)(candle.soulLimit * 1.25f), candle.soulLimit + 1);
            }
        }

        public override void ModifyValue(ref float valueMult)
        {
            valueMult += 0.25f;
        }
    }
}