namespace Aequus;

public partial class AequusPlayer {
    public static bool EquipmentModifierUpdate;

    private void PostUpdateEquips_EmpoweredEquipAbilities() {
        //if (accCrownOfBloodItemClone == null || accCrownOfBloodItemClone.IsAir || !EquipBoostDatabase.Instance.Entries.IndexInRange(accCrownOfBloodItemClone.type)) {
        //    return;
        //}

        //var empowerment = accCrownOfBloodItemClone.GetGlobalItem<EquipBoostGlobalItem>().equipEmpowerment;
        //var entry = EquipBoostDatabase.Instance.Entries[accCrownOfBloodItemClone.type];
        //if (empowerment == null || !empowerment.HasAnyBoost || entry.CustomEquipUpdate == null) {
        //    return;
        //}
        //entry.CustomEquipUpdate(Player, accCrownOfBloodItemClone);
    }
}
