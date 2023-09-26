﻿using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Classless;
public class GenericDamageClassNoCrit : DamageClass {
    public override StatInheritanceData GetModifierInheritance(DamageClass damageClass) {
        return StatInheritanceData.None;
    }

    public override bool UseStandardCritCalcs => false;
}