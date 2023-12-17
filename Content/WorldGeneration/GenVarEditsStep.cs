using Aequus.Common.WorldGeneration;
using Aequus.Content.Configuration;
using Terraria.ID;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace Aequus.Content.WorldGeneration;
public class GenVarEditsStep : AequusGenStep {
    public override string InsertAfter => "Reset";

    public override void Apply(GenerationProgress progress, GameConfiguration config) {
        if (GenVars.hellChestItem != null) {
            if (VanillaChangesConfig.Instance.MoveTreasureMagnet) {
                CollectionHelper.Remove(ref GenVars.hellChestItem, ItemID.TreasureMagnet);
            }
        }
    }
}