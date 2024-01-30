namespace Aequus.Common.ItemPrefixes;

public class InstancedPrefix : ModPrefix {
    public readonly record struct StatModifiers(System.Single? DamageMultiplier = default, System.Single? KnockbackMultiplier = default, System.Single? UseTimeMultiplier = default, System.Single? ScaleMultiplier = default, System.Single? ShootSpeedMultiplier = default, System.Single? ManaMultiplier = default, System.Int32? CritBonus = default);

    private readonly System.String _name;

    public override System.String Name => _name;

    public readonly System.Single priceMultiplier;
    public readonly StatModifiers statModifiers;

    // ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus

    public InstancedPrefix(System.String name, System.Single priceMultiplier, StatModifiers statModifiers = default) {
        _name = name;
        this.priceMultiplier = priceMultiplier;
        this.statModifiers = statModifiers;
    }

    public override void SetStats(ref System.Single damageMult, ref System.Single knockbackMult, ref System.Single useTimeMult, ref System.Single scaleMult, ref System.Single shootSpeedMult, ref System.Single manaMult, ref System.Int32 critBonus) {
        damageMult = statModifiers.DamageMultiplier ?? damageMult;
        knockbackMult = statModifiers.KnockbackMultiplier ?? knockbackMult;
        scaleMult = statModifiers.ScaleMultiplier ?? scaleMult;
        shootSpeedMult = statModifiers.ShootSpeedMultiplier ?? shootSpeedMult;
        manaMult = statModifiers.ManaMultiplier ?? manaMult;
        critBonus = statModifiers.CritBonus ?? critBonus;
    }

    public override void ModifyValue(ref System.Single valueMult) {
        valueMult = priceMultiplier;
    }
}