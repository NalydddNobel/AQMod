using System.Collections.Generic;
using Terraria.WorldBuilding;

namespace Aequus.Common.WorldGeneration;

public class WorldGenSystem : ModSystem {
    public static readonly List<AequusGenStep> GenerationSteps = new();

    public static readonly HashSet<System.Int32> PlacedItems = new();

    public override void Unload() {
        GenerationSteps.Clear();
    }

    public override void PreWorldGen() {
        PlacedItems.Clear();
    }

    public override void ModifyWorldGenTasks(List<GenPass> tasks, ref System.Double totalWeight) {
        foreach (var step in GenerationSteps) {
            System.String sortAfter = step.InsertAfter;
            System.Int32 index = tasks.FindIndex((pass) => pass.Name.Equals(sortAfter));
            if (index == -1) {
                step.EmergencyOnStepNotFound(tasks);
            }
            else {
                step.InsertStep(index + 1, tasks);
            }
        }
    }
}