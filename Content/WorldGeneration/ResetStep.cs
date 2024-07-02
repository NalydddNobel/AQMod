using Aequus.Content.VanillaChanges;
using Aequus.Core.WorldGeneration;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace Aequus.Content.WorldGeneration;

public sealed class ResetStep : AequusGenStep {
    public override string InsertAfter => "Reset";

    public override void Apply(GenerationProgress progress, GameConfiguration config) {
        if (GenVars.hellChestItem != null) {
            TreasureMagnetChanges.RemoveTreasureMagnetFromHellChestArray();
        }
        foreach (var genStep in WorldGenSystem.GenerationSteps) {
            genStep.Reset();
        }
    }
}