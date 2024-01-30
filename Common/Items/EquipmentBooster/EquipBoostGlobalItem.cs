namespace Aequus.Common.Items.EquipmentBooster;

public class EquipBoostGlobalItem : GlobalItem {
    public override System.Boolean AppliesToEntity(Item entity, System.Boolean lateInstantiation) {
        return !entity.vanity && (entity.accessory || entity.headSlot > -1 || entity.bodySlot > -1 || entity.legSlot > -1);
    }

    public override System.Boolean InstancePerEntity => true;

    public EquipBoostInfo equipEmpowerment = null;

    public override void SetDefaults(Item entity) {
        equipEmpowerment = null;
    }

    public override void UpdateInventory(Item item, Player player) {
        if (!AequusPlayer.EquipmentModifierUpdate) {
            equipEmpowerment = null;
        }
    }

    public override void Update(Item item, ref System.Single gravity, ref System.Single maxFallSpeed) {
        equipEmpowerment = null;
    }

    public override void UpdateEquip(Item item, Player player) {
    }
}