﻿using Aequus.Common.Items.EquipmentBooster;
using Terraria;

namespace Aequus;

public partial class AequusPlayer {
    private void PostUpdateEquips_EmpoweredEquipAbilities() {
        if (accCrownOfBloodItemClone == null || accCrownOfBloodItemClone.IsAir || !EquipBoostDatabase.Instance.Entries.IndexInRange(accCrownOfBloodItemClone.type)) {
            return;
        }

        var empowerment = accCrownOfBloodItemClone.GetGlobalItem<EquipBoostGlobalItem>().equipEmpowerment;
        var entry = EquipBoostDatabase.Instance.Entries[accCrownOfBloodItemClone.type];
        if (empowerment == null || !empowerment.HasAnyBoost || entry.CustomEquipUpdate == null) {
            return;
        }
        entry.CustomEquipUpdate(Player, accCrownOfBloodItemClone);
    }
}
