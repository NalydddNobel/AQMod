using Aequus.Items.Weapons.Summon.Necro.Candles;
using System;
using Terraria;

namespace Aequus.Items.Prefixes.SoulCandles
{
    public class EnlightenedPrefix : SoulCandlePrefix
    {
        public override void Apply(Item item)
        {
            if (item.ModItem is BaseSoulCandle candle)
            {
                candle.ClearPrefix();
                candle.soulCost = Math.Max((int)(candle.soulCost * 1.25f), candle.soulCost + 1);
            }
        }

        public override void ModifyValue(ref float valueMult)
        {
            valueMult -= 0.25f;
        }
    }
}