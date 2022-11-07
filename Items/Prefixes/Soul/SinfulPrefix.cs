using Aequus.Items.Weapons;
using System;
using Terraria;

namespace Aequus.Items.Prefixes.Soul
{
    public class SinfulPrefix : SoulWeaponPrefix
    {
        public override void Apply(Item item)
        {
            if (item.ModItem is SoulWeaponBase candle)
            {
                candle.ClearSoulFields();
                candle.soulCost = Math.Min((int)(candle.soulCost * 0.9f), candle.soulCost - 1);
            }
        }

        public override void ModifyValue(ref float valueMult)
        {
            valueMult += 0.1f;
        }
    }
}