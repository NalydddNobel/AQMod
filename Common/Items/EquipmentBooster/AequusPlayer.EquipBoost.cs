using Aequus.Common.Items.EquipmentBooster;
using Terraria;

namespace Aequus;

public partial class AequusPlayer {
    public EquipBoostManager equipModifiers;

    private void Initalize_EquipModifiers() {
        equipModifiers ??= new();
    }

    private void ResetEffects_EquipModifiers() {
        (equipModifiers ??= new()).ResetEffects();
    }

    private void UpdateEquips_UpdateEmpoweredAccessories() {
        for (int i = Player.SupportedSlotsArmor; i < Player.SupportedSlotSets; i++) {

            var accessoryItem = Player.armor[i];
            if (accessoryItem.IsAir)
                continue;

            var modifier = equipModifiers.GetVanilla(i);
            if (modifier.Boost == EquipBoostType.None) {

                continue;
            }

            modifier.ApplyModifier(accessoryItem, Player, this, hideVisual: Player.hideVisibleAccessory[i]);
        }
    }

    private void PostUpdateEquips_UpdateEmpoweredArmors() {
        for (int i = 0; i < Player.SupportedSlotsArmor; i++) {
            var armorItem = Player.armor[2];
            if (armorItem.IsAir)
                continue;

            var modifier = equipModifiers.GetVanilla(i);
            if (modifier.Boost == EquipBoostType.None) {
                continue;
            }

            modifier.ApplyModifier(armorItem, Player, this, hideVisual: false);
        }
    }

    private void PostUpdateEquips_EmpoweredEquipAbilities() {
        if (accCrownOfBloodItemClone == null || accCrownOfBloodItemClone.IsAir) {
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
