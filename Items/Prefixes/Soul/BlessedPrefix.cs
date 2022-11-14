using Aequus.Items.Weapons;
using System;
using Terraria;
using Terraria.Utilities;

namespace Aequus.Items.Prefixes.Soul
{
    public class BlessedPrefix : SoulWeaponPrefix
    {
        public override void Apply(Item item)
        {
            if (item.ModItem is SoulGemWeaponBase soulWeapon)
            {
                soulWeapon.ClearSoulFields();
                soulWeapon.tier = Math.Min(soulWeapon.tier + 1, SoulGemWeaponBase.MaxTier);
            }
        }

        public override bool CanChoose(Item item, SoulGemWeaponBase soulGem, UnifiedRandom rand)
        {
            return soulGem.OriginalTier < SoulGemWeaponBase.MaxTier;
        }

        public override void ModifyValue(ref float valueMult)
        {
            valueMult -= 0.1f;
        }
    }
}