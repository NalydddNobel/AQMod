using Terraria.Localization;

namespace Aequu2.Core.Entities.DamageClasses;

public class GenericDamageClassNoCrit : DamageClass {
    public override LocalizedText DisplayName => Generic.DisplayName;

    public override bool UseStandardCritCalcs => false;
}