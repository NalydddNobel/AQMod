using System;

namespace Aequus.Common.Items.EquipmentBooster;

[Flags]
public enum EquipBoostType : byte {
    None = 0,
    Defense = 1,
    Abilities = 2,
}