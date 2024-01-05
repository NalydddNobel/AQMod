using Aequus.Common.Tiles;
using Aequus.Content.WorldGeneration;
using System.Collections.Generic;
using Terraria.ObjectData;
using Terraria.WorldBuilding;

namespace Aequus.Common.WorldGeneration;

public class WorldGenSystem : ModSystem {
    public static readonly List<AequusGenStep> GenerationSteps = new();

    public static readonly HashSet<int> PlacedItems = new();

    public override void Unload() {
        GenerationSteps.Clear();
    }

    public override void PreWorldGen() {
        PlacedItems.Clear();
    }

    public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight) {
        foreach (var step in GenerationSteps) {
            string sortAfter = step.InsertAfter;
            int index = tasks.FindIndex((pass) => pass.Name.Equals(sortAfter));
            if (index == -1) {
                step.EmergencyOnStepNotFound(tasks);
            }
            else {
                step.InsertStep(index + 1, tasks);
            }
        }
    }
}