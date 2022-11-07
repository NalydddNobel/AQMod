using Aequus.Items.Weapons;
using System;
using Terraria;

namespace Aequus.Items.Prefixes.Soul
{
    public class DivinePrefix : SoulWeaponPrefix
    {
        public override void Apply(Item item)
        {
            if (item.ModItem is SoulWeaponBase candle)
            {
                candle.ClearSoulFields();
                candle.soulLimit = Math.Min((int)(candle.soulLimit * 0.75f), candle.soulLimit - 1);
                candle.soulCost = Math.Max((int)(candle.soulCost * 1.25f), candle.soulCost + 1);
            }
        }

        public override void ModifyValue(ref float valueMult)
        {
            valueMult -= 0.4f;
        }
    }
}