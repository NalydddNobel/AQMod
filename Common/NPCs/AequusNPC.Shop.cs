﻿using Aequus.Content.Fishing.FishingPoles;

namespace Aequus.Common.NPCs;

public partial class AequusNPC {
    public override void ModifyShop(NPCShop shop) {
        switch ((shop.NpcType, shop.Name)) {
            case (NPCID.Steampunker, "Shop"):
                shop.Add(FishingPoleLoader.SteampunkersRod.Type, Condition.MoonPhasesEven, Condition.NpcIsPresent(NPCID.Angler));
                break;
        }
    }
}
