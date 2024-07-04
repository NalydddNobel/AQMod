using Terraria.Localization;

namespace AequusRemake.Core.Entities.DamageClasses;

public class OmniDamageClass : DamageClass {
    public override LocalizedText DisplayName => Generic.DisplayName;

    public override StatInheritanceData GetModifierInheritance(DamageClass damageClass) {
        return StatInheritanceData.Full;
    }
}
