using Aequus.Items.Weapons;
using System;
using Terraria;

namespace Aequus.Items.Prefixes.Soul
{
    public class EnlightenedPrefix : SoulWeaponPrefix
    {
        public override void Apply(Item item)
        {
            if (item.ModItem is SoulWeaponBase candle)
            {
                candle.ClearSoulFields();
                candle.soulCost = Math.Max((int)(candle.soulCost * 1.25f), candle.soulCost + 1);
            }
        }

        public override void ModifyValue(ref float valueMult)
        {
            valueMult -= 0.25f;
        }
    }
}