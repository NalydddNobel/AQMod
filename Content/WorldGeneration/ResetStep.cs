using Aequu2.Content.Systems;
using Aequu2.Content.VanillaChanges;
using Aequu2.Core.Components;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace Aequu2.Content.WorldGeneration;

public sealed class ResetStep : AGenStep {
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