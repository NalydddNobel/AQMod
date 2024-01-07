using Aequus.Content.Fishing.FishingPoles.Steampunker;

namespace Aequus.Common.NPCs;

public partial class AequusNPC {
    public override void ModifyShop(NPCShop shop) {
        switch ((shop.NpcType, shop.Name)) {
            case (NPCID.Steampunker, "Shop"):
                shop.Add(ModContent.ItemType<SteampunkerFishingPole>(), Condition.MoonPhasesEven, Condition.NpcIsPresent(NPCID.Angler));
                break;
        }
    }
}
