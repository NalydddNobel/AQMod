namespace Aequus.Common.DamageClasses;

public class OmniDamageClassNoCrit : OmniDamageClass {
    public override bool UseStandardCritCalcs => false;
}
