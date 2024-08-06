using System;
using Terraria.WorldBuilding;

namespace Aequus.Systems.WorldGeneration;

public static class GenerationExtensions {
    public static void SetProgress(this GenerationProgress progress, double progressValue, double prevProgress = 0f, double wantedProgress = 1f) {
        if (progress != null) {
            progress.Value = prevProgress + (wantedProgress - prevProgress) * Math.Clamp(progressValue, 0f, 1f);
        }
    }
}
