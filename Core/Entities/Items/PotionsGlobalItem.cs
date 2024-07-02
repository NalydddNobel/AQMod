using Aequus.DataSets;

namespace Aequus.Core.Entities.Items;

public class PotionsGlobalItem : GlobalItem {
    public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
        return ItemDataSet.Potions.Contains(entity.type);
    }

    public override void SetDefaults(Item entity) {
        entity.AllowReforgeForStackableItem = true;
    }
}
