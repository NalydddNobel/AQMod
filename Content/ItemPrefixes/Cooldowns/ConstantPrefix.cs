using Aequus.Common.ItemPrefixes;

namespace Aequus.Content.ItemPrefixes.Cooldowns;
public class ConstantPrefix : CooldownPrefixBase {
    public override float CooldownMultiplier => 0.80f;

    public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus) {
        useTimeMult = 0.75f;
        manaMult = 0.75f;
    }

    public override void ModifyValue(ref float valueMult) {
        valueMult = 1.66f;
    }
}