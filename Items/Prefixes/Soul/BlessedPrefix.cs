using Aequus.Items.Weapons;
using System;
using Terraria;

namespace Aequus.Items.Prefixes.Soul
{
    public class BlessedPrefix : SoulWeaponPrefix
    {
        public override void Apply(Item item)
        {
            if (item.ModItem is SoulWeaponBase soul)
            {
                soul.ClearSoulFields();
                soul.soulCost = Math.Max((int)(soul.soulCost * 1.1f), soul.soulCost + 1);
            }
        }

        public override void ModifyValue(ref float valueMult)
        {
            valueMult -= 0.1f;
        }
    }
}