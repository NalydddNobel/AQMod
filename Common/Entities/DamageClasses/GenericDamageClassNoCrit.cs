using Terraria.Localization;

namespace Aequus.Common.Entities.DamageClasses;

public class GenericDamageClassNoCrit : DamageClass {
    public override LocalizedText DisplayName => Generic.DisplayName;

    public override bool UseStandardCritCalcs => false;
}