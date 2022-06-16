using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Summon
{
    public class NecromancyDamageClass : DamageClass
    {
        protected override string DisplayNameInternal => Language.GetTextValue("LegacyTooltip.53").Substring(1);

        public override bool UseStandardCritCalcs => false;

        public override StatInheritanceData GetModifierInheritance(DamageClass damageClass)
        {
            if (damageClass == Summon)
            {
                return StatInheritanceData.Full;
            }
            return base.GetModifierInheritance(damageClass);
        }

        public override bool GetEffectInheritance(DamageClass damageClass)
        {
            return damageClass == Summon;
        }

        public override bool ShowStatTooltipLine(Player player, string lineName)
        {
            return lineName != "CritChance" && lineName != "Speed";
        }
    }
}