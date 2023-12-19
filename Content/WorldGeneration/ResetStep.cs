﻿using Aequus.Common.WorldGeneration;
using Aequus.Content.Configuration;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace Aequus.Content.WorldGeneration;

public sealed class ResetStep : AequusGenStep {
    public override string InsertAfter => "Reset";

    public override void Apply(GenerationProgress progress, GameConfiguration config) {
        if (GenVars.hellChestItem != null) {
            if (VanillaChangesConfig.Instance.MoveTreasureMagnet) {
                EnumerableHelper.Remove(ref GenVars.hellChestItem, ItemID.TreasureMagnet);
            }
        }
        foreach (var genStep in WorldGenSystem.GenerationSteps) {
            genStep.Reset();
        }
    }
}