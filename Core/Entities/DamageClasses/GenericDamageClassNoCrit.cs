using Terraria.Localization;

namespace AequusRemake.Core.Entities.DamageClasses;

public class GenericDamageClassNoCrit : DamageClass {
    public override LocalizedText DisplayName => Generic.DisplayName;

    public override bool UseStandardCritCalcs => false;
}