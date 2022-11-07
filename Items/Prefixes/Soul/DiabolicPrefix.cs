using Aequus.Items.Weapons;
using System;
using Terraria;

namespace Aequus.Items.Prefixes.Soul
{
    public class DiabolicPrefix : SoulWeaponPrefix
    {
        public override void Apply(Item item)
        {
            if (item.ModItem is SoulWeaponBase candle)
            {
                candle.ClearSoulFields();
                candle.soulLimit = Math.Max((int)(candle.soulLimit * 1.25f), candle.soulLimit + 1);
            }
        }

        public override void ModifyValue(ref float valueMult)
        {
            valueMult += 0.25f;
        }
    }
}