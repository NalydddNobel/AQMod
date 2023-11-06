using Aequus.Common.ItemPrefixes;

namespace Aequus.Content.ItemPrefixes.Cooldowns;
public class PunctualPrefix : CooldownPrefixBase {
    public override float CooldownMultiplier => 0.90f;

    public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus) {
        damageMult = 1.05f;
        manaMult = 0.9f;
        knockbackMult = 1.25f;
    }

    public override void ModifyValue(ref float valueMult) {
        valueMult = 1.5f;
    }
}