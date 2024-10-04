namespace Aequus.Content.Items.Accessories.AccessoryDamageEarring;

public class AccessoryDamageGlobalItem : GlobalItem {
    public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
        return entity.accessory;
    }

    public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage) {
        if (item.accessory && player.TryGetModPlayer(out AequusPlayer aequus)) {
            damage = damage.CombineWith(aequus.accessoryDamage);
        }
    }
}
