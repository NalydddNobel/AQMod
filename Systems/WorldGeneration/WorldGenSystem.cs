using System;
using System.Collections.Generic;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace Aequus.Systems.WorldGeneration;

public class WorldGenSystem : ModSystem {
    public static WorldGenSystem Instance => ModContent.GetInstance<WorldGenSystem>();

    public readonly List<AGenStep> GenerationSteps = [];

    public readonly HashSet<int> PlacedItems = [];

    public Action? OnReset { get; internal set; }

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

public sealed class ResetStep : AGenStep {
    public override string InsertAfter => "Reset";

    public override void Apply(GenerationProgress progress, GameConfiguration config) {
        WorldGenSystem.Instance.OnReset?.Invoke();
        foreach (var genStep in WorldGenSystem.Instance.GenerationSteps) {
            genStep.OnGenerationReset();
        }
    }
}