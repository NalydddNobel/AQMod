namespace AequusRemake.Core.ContentGeneration;

internal class InstancedModPrefix : ModPrefix {
    public readonly record struct StatModifiers(float? DamageMultiplier = default, float? KnockbackMultiplier = default, float? UseTimeMultiplier = default, float? ScaleMultiplier = default, float? ShootSpeedMultiplier = default, float? ManaMultiplier = default, int? CritBonus = default);

    private readonly string _name;

    public override string Name => _name;

    public readonly float priceMultiplier;
    public readonly StatModifiers statModifiers;

    // ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus

    public InstancedModPrefix(string name, float priceMultiplier, StatModifiers statModifiers = default) {
        _name = name;
        this.priceMultiplier = priceMultiplier;
        this.statModifiers = statModifiers;
    }

    public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus) {
        damageMult = statModifiers.DamageMultiplier ?? damageMult;
        knockbackMult = statModifiers.KnockbackMultiplier ?? knockbackMult;
        scaleMult = statModifiers.ScaleMultiplier ?? scaleMult;
        shootSpeedMult = statModifiers.ShootSpeedMultiplier ?? shootSpeedMult;
        manaMult = statModifiers.ManaMultiplier ?? manaMult;
        critBonus = statModifiers.CritBonus ?? critBonus;
    }

    public override void ModifyValue(ref float valueMult) {
        valueMult = priceMultiplier;
    }
}