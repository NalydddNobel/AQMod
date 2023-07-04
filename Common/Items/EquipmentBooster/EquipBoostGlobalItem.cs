using Terraria;
using Terraria.ModLoader;

namespace Aequus.Common.Items.EquipmentBooster;

public class EquipBoostGlobalItem : GlobalItem {
    public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
        return entity.accessory || entity.headSlot > -1 || entity.bodySlot > -1 || entity.legSlot > -1;
    }

    public override bool InstancePerEntity => true;

    public EquipBoostInfo equipEmpowerment = null;

    public override void SetDefaults(Item entity) {
        equipEmpowerment = null;
    }

    public override void UpdateInventory(Item item, Player player) {
        if (!AequusPlayer.EquipmentModifierUpdate) {
            equipEmpowerment = null;
        }
    }

    public override void Update(Item item, ref float gravity, ref float maxFallSpeed) {
        equipEmpowerment = null;
    }

    public override void UpdateEquip(Item item, Player player) {
        if (!AequusPlayer.EquipmentModifierUpdate) {
            equipEmpowerment = null;
        }
    }
}