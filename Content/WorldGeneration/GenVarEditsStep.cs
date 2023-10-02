using Aequus.Common.WorldGeneration;
using Aequus.Content.Configuration;
using System.Collections.Generic;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Aequus.Content.WorldGeneration;
public class GenVarEditsStep : AequusGenStep {
    public override string InsertAfter => "Reset";

    public override void Apply(GenerationProgress progress, GameConfiguration config) {
        if (ServerConfig.Instance.MoveTreasureMagnet && GenVars.hellChestItem != null) {
            // Remove Treasure Magnet from Shadow Chest loot
            List<int> list = new(GenVars.hellChestItem);
            list.Remove(ItemID.TreasureMagnet);
            GenVars.hellChestItem = list.ToArray();
        }
    }
}