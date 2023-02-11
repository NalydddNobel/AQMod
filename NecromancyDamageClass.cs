using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus
{
    public class NecromancyDamageClass : DamageClass
    {
        public static NecromancyDamageClass Instance { get; private set; }
        public static Color NecromancyDamageColor = new Color(200, 120, 230);

        protected override string DisplayNameInternal => Language.GetTextValue("LegacyTooltip.53")[1..];

        public override bool UseStandardCritCalcs => false;

        public override void Load()
        {
            Instance = this;
        }

        public override void Unload()
        {
            Instance = null;
        }

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