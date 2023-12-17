using Aequus.Content.UI.SlotDecals;
using Terraria;
using Terraria.UI;

namespace Aequus.Content.Equipment.Accessories.AccCrowns.Blood;

public class BloodCrownSlotDecal : SlotDecal {
    public override bool CanDraw(int slot, int context) {
        return slot == BloodCrown.SlotId && context == ItemSlot.Context.EquipAccessory && Main.LocalPlayer.GetModPlayer<BloodCrownPlayer>().accBloodCrown;
    }
}