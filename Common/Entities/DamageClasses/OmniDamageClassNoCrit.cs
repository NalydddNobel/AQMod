namespace Aequus.Common.Entities.DamageClasses;

public class OmniDamageClassNoCrit : OmniDamageClass {
    public override bool UseStandardCritCalcs => false;
}
