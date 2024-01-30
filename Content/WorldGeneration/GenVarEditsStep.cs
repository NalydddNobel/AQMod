using Aequus.Common.WorldGeneration;
using Aequus.Content.VanillaChanges;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace Aequus.Content.WorldGeneration;
public class GenVarEditsStep : AequusGenStep {
    public override System.String InsertAfter => "Reset";

    public override void Apply(GenerationProgress progress, GameConfiguration config) {
        if (GenVars.hellChestItem != null) {
            TreasureMagnetChanges.RemoveTreasureMagnetFromHellChestArray();
        }
    }
}