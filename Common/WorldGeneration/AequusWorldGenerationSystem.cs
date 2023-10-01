using System.Collections.Generic;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Aequus.Common.WorldGeneration;

public class AequusWorldGenerationSystem : ModSystem {
    public static List<AequusGenStep> GenerationSteps = new();

    public override void Unload() {
        GenerationSteps.Clear();
    }

    public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight) {
        foreach (var step in GenerationSteps) {
            string sortAfter = step.InsertAfter;
            int index = tasks.FindIndex((pass) => pass.Name.Equals(sortAfter));
            if (index == -1) {
                step.EmergencyOnStepNotFound(tasks);
            }
            else {
                step.InsertStep(index, tasks);
            }
        }
    }
}