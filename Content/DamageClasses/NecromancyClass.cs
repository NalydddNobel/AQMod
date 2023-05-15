using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Content.DamageClasses {
    public class NecromancyClass : DamageClass {
        public static readonly Color NecromancyDamageColor = new Color(200, 120, 230);
        public static readonly Color NecromancyCritColor = new Color(200, 120, 230) * 1.25f;

        public override bool UseStandardCritCalcs => false;

        public override StatInheritanceData GetModifierInheritance(DamageClass damageClass) {
            if (damageClass == Summon || damageClass == Magic) {
                return StatInheritanceData.Full;
            }
            return base.GetModifierInheritance(damageClass);
        }

        public override bool GetEffectInheritance(DamageClass damageClass) {
            return damageClass == Summon || damageClass == Magic || damageClass is NecromancyClass;
        }

        public override bool ShowStatTooltipLine(Player player, string lineName) {
            return lineName != "CritChance";
        }
    }
}