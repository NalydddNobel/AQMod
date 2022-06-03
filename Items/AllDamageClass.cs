using Terraria.ModLoader;

namespace Aequus.Items
{
    public class AllDamageClass : DamageClass
    {
        public static DamageClass Instance => new AllDamageClass();

        public override bool UseStandardCritCalcs => false;

        public override StatInheritanceData GetModifierInheritance(DamageClass damageClass)
        {
            return StatInheritanceData.Full;
        }
    }
}