using System;

namespace Aequus.Common.Items.EquipmentBooster;

[Flags]
public enum EquipBoostType : Byte {
    None = 0,
    Defense = 1,
    Abilities = 2,
}