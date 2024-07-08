using AequusRemake.Core.Components;
using System.Collections.Generic;
using Terraria.WorldBuilding;

namespace AequusRemake.Systems;

public class WorldGenSystem : ModSystem {
    public static readonly List<AGenStep> GenerationSteps = [];

    public static readonly HashSet<int> PlacedItems = [];

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