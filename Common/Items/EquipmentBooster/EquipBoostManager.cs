using Terraria;

namespace Aequus.Common.Items.EquipmentBooster;

public class EquipBoostManager {
    private EquipBoostInfo[] vanilla;

    public EquipBoostManager() {
        vanilla = new EquipBoostInfo[Player.SupportedSlotSets];
        for (int i = 0; i < Player.SupportedSlotSets; i++) {
            vanilla[i] = new(i);
        }
    }

    public void ResetEffects() {
        for (int i = 0; i < vanilla.Length; i++) {
            vanilla[i].ResetEffects();
        }
    }

    public ref EquipBoostInfo Head() {
        return ref vanilla[0];
    }
    public ref EquipBoostInfo Body() {
        return ref vanilla[1];
    }
    public ref EquipBoostInfo Legs() {
        return ref vanilla[2];
    }
    public ref EquipBoostInfo FirstUnempoweredAccessory(EquipBoostType parameters) {
        for (int slot = Player.SupportedSlotsArmor; slot < Player.SupportedSlotSets; slot++) {
            if ((vanilla[slot].Boost & parameters) == parameters) {
                continue;
            }

            return ref vanilla[slot];
        }
        return ref vanilla[3];
    }
    public ref EquipBoostInfo GetVanilla(int slot) {
        return ref vanilla[slot];
    }
}