﻿namespace Aequus.Common.ItemModifiers.Cooldowns;

public class TardyPrefix : CooldownPrefixBase {
    public override float CooldownMultiplier => 1.2f;

    public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus) {
        useTimeMult = 1.2f;
    }

    public override void ModifyValue(ref float valueMult) {
        valueMult = 0.7f;
    }
}