using Aequus.Items.Weapons;
using Terraria;

namespace Aequus.Items.Prefixes.Soul
{
    public class DiabolicPrefix : SoulWeaponPrefix
    {
        public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus)
        {
            damageMult += 0.1f;
        }

        public override void Apply(Item item)
        {
            if (item.ModItem is SoulGemWeaponBase soulWeapon)
            {
                soulWeapon.ClearSoulFields();
            }
        }

        public override void ModifyValue(ref float valueMult)
        {
            valueMult += 0.25f;
        }
    }
}