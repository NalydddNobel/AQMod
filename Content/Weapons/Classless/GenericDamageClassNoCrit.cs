namespace Aequus.Content.Weapons.Classless;
public class GenericDamageClassNoCrit : DamageClass {
    public override StatInheritanceData GetModifierInheritance(DamageClass damageClass) {
        return StatInheritanceData.None;
    }

    public override System.Boolean UseStandardCritCalcs => false;
}