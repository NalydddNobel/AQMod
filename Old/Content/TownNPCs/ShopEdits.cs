using Aequus.Old.Content.Tiles.Herbs.PlanterBoxes;

namespace Aequus.Old.Content.TownNPCs;

public class ShopEdits : GlobalNPC {
    public override void ModifyShop(NPCShop shop) {
        // We don't modify any alternative shops yet.
        if (shop.Name != "Shop") {
            return;
        }

        switch (shop.NpcType) {
            case NPCID.Dryad: {
                    // Add new planter boxes to the Dryad's shop.
                    foreach (var planterBox in ModContent.GetInstance<PlanterBox>().RegisteredPlanterBoxItems) {
                        shop.InsertAfter(planterBox.ShopSortingItemIdTarget, planterBox.ModItem.Type, planterBox.SellCondition);
                    }
                }
                break;
        }
    }
}
