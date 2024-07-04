using AequusRemake.Content.Systems;
using AequusRemake.Content.VanillaChanges;
using AequusRemake.Core.Components;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace AequusRemake.Content.WorldGeneration;

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