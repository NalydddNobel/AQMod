using Aequus.Items.Weapons;
using System;
using Terraria;
using Terraria.Utilities;

namespace Aequus.Items.Prefixes.Soul
{
    public class SinfulPrefix : SoulWeaponPrefix
    {
        public override void Apply(Item item)
        {
            if (item.ModItem is SoulGemWeaponBase soulWeapon)
            {
                soulWeapon.ClearSoulFields();
                soulWeapon.tier = Math.Max(soulWeapon.tier - 1, SoulGemWeaponBase.MaxTier);
            }
        }

        public override bool CanChoose(Item item, SoulGemWeaponBase soulGem, UnifiedRandom rand)
        {
            return soulGem.OriginalTier > SoulGemWeaponBase.MinTier;
        }

        public override void ModifyValue(ref float valueMult)
        {
            valueMult += 0.1f;
        }
    }
}