using Aequus.Content.Fishing.FishingPoles;
using Aequus.Content.Tiles.Meadow;

namespace Aequus.Common.NPCs;

public class ShopEdits : GlobalNPC {
    public override void ModifyShop(NPCShop shop) {
        switch ((shop.NpcType, shop.Name)) {
            case (NPCID.Dryad, "Shop"):
                shop.InsertAfter(ItemID.GrassSeeds, 
                    ModContent.GetInstance<MeadowGrass>().MeadowGrassSeeds.Type, Condition.InSpace);

#if !DEBUG
                // Add new planter boxes to the Dryad's shop.
                foreach (var planterBox in ModContent.GetInstance<Old.Content.Tiles.Herbs.PlanterBoxes.PlanterBox>().RegisteredPlanterBoxItems) {
                    shop.InsertAfter(planterBox.ShopSortingItemIdTarget, planterBox.ModItem.Type, planterBox.SellCondition);
                }
#endif
                break;

            case (NPCID.Mechanic, "Shop"):
#if !DEBUG
                shop.Add(ModContent.ItemType<Old.Content.Items.Weapons.Sentries.PhysicistSentry.PhysicistSentry>(), Condition.RemixWorld);
#endif
                break;

            case (NPCID.Steampunker, "Shop"):
                shop.Add(ModContent.ItemType<SteampunkerFishingPole>(), Condition.MoonPhasesEven, Condition.NpcIsPresent(NPCID.Angler));
                break;
        }
    }
}
