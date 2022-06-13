using Aequus.Items.Weapons.Summon.Candles;
using System;
using Terraria;

namespace Aequus.Content.Prefixes.SoulCandles
{
    public class PraisedPrefix : SoulCandlePrefix
    {
        public override void Apply(Item item)
        {
            if (item.ModItem is SoulCandle candle)
            {
                candle.ClearPrefix();
                candle.soulLimit = Math.Max((int)(candle.soulLimit * 0.75f), candle.soulLimit - 1);
            }
        }

        public override void ModifyValue(ref float valueMult)
        {
            valueMult -= 0.25f;
        }
    }
}