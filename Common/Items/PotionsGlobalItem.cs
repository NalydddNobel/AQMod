using Aequus.Content.DataSets;

namespace Aequus.Common.Items;

public class PotionsGlobalItem : GlobalItem {
    public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
        return ItemSets.Potions.Contains(entity.type);
    }

    public override void SetDefaults(Item entity) {
        entity.AllowReforgeForStackableItem = true;
    }
}
