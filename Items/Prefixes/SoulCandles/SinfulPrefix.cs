﻿using Aequus.Items.Weapons.Summon.Necro.Candles;
using System;
using Terraria;

namespace Aequus.Items.Prefixes.SoulCandles
{
    public class SinfulPrefix : SoulCandlePrefix
    {
        public override void Apply(Item item)
        {
            if (item.ModItem is BaseSoulCandle candle)
            {
                candle.ClearPrefix();
                candle.useSouls = Math.Max((int)(candle.useSouls * 0.75f), candle.useSouls - 1);
            }
        }

        public override void ModifyValue(ref float valueMult)
        {
            valueMult += 0.1f;
        }
    }
}