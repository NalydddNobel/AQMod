using Aequus.Items.Weapons.Summon.Candles;
using System;
using Terraria;

namespace Aequus.Content.Prefixes.SoulCandles
{
    public class EnlightenedPrefix : SoulCandlePrefix
    {
        public override void Apply(Item item)
        {
            if (item.ModItem is SoulCandle candle)
            {
                candle.ClearPrefix();
                candle.useSouls = Math.Max((int)(candle.useSouls * 1.25f), candle.useSouls + 1);
            }
        }

        public override void ModifyValue(ref float valueMult)
        {
            valueMult -= 0.25f;
        }
    }
}