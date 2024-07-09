using AequusRemake.Content.Tiles.Meadow;
using AequusRemake.Systems.Fishing.FishingPoles;

namespace AequusRemake.Core.Entities.NPCs;

public class ShopEdits : GlobalNPC {
    public override void ModifyShop(NPCShop shop) {
        switch ((shop.NpcType, shop.Name)) {
            case (NPCID.ArmsDealer, "Shop"):
                shop.Add(ModContent.ItemType<Content.Items.Weapons.Ranged.GasCan.GasCan>(), Condition.PlayerCarriesItem(ModContent.ItemType<Content.Items.Weapons.Ranged.GasCan.GasCan>()));
                break;

            case (NPCID.Dryad, "Shop"):
                shop.InsertAfter(ItemID.GrassSeeds,
                    ModContent.GetInstance<MeadowGrass>().MeadowGrassSeeds.Type, Condition.InSpace);

                // Add new planter boxes to the Dryad's shop.
                //foreach (var planterBox in ModContent.GetInstance<Old.Content.Tiles.Herbs.PlanterBoxes.PlanterBox>().RegisteredPlanterBoxItems) {
                //    shop.InsertAfter(planterBox.ShopSortingItemIdTarget, planterBox.ModItem.Type, planterBox.SellCondition);
                //}
                break;

            case (NPCID.Mechanic, "Shop"):
                //shop.Add(ModContent.ItemType<Old.Content.Items.Weapons.Summon.Sentries.PhysicistSentry.PhysicistSentry>(), Condition.RemixWorld);
                break;

            case (NPCID.Steampunker, "Shop"):
                shop.Add(ModContent.ItemType<SteampunkerFishingPole>(), Condition.MoonPhasesEven, Condition.NpcIsPresent(NPCID.Angler));
                break;
        }
    }
}
