using AequusRemake.DataSets;

namespace AequusRemake.Core.Entities.Items;

public class PotionsGlobalItem : GlobalItem {
    public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
        return ItemDataSet.Potions.Contains(entity.type);
    }

    public override void SetDefaults(Item entity) {
        entity.AllowReforgeForStackableItem = true;
    }
}
