namespace Aequus.Common.ItemModifiers.Cooldowns;
public class OrganizedPrefix : CooldownPrefixBase {
    public override float CooldownMultiplier => 0.95f;

    public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus) {
        damageMult = 1.05f;
        useTimeMult = 0.9f;
        knockbackMult = 0.9f;
    }

    public override void ModifyValue(ref float valueMult) {
        valueMult = 1.15f;
    }
}