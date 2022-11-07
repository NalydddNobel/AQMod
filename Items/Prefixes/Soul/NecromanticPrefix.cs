using Aequus.Items.Weapons;
using System;
using Terraria;

namespace Aequus.Items.Prefixes.Soul
{
    public class NecromanticPrefix : SoulWeaponPrefix
    {
        public override void Apply(Item item)
        {
            if (item.ModItem is SoulWeaponBase candle)
            {
                candle.ClearSoulFields();
                candle.soulLimit = Math.Max((int)(candle.soulLimit * 1.25f), candle.soulLimit + 1);
                candle.soulCost = Math.Min((int)(candle.soulCost * 0.75f), candle.soulCost - 1);
            }
        }

        public override void ModifyValue(ref float valueMult)
        {
            valueMult += 0.4f;
        }
    }
}