using Aequus.Core.Autoloading;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.Utilities;
using Terraria.WorldBuilding;

namespace Aequus.Common.WorldGeneration;

public abstract class AequusGenStep : ModType, ILocalizedModType, IPostSetupContent {
    public abstract string InsertAfter { get; }

    public double Weight { get; private set; }

    public static UnifiedRandom Random => WorldGen.genRand;

    protected virtual double GenWeight { get; }

    public string LocalizationCategory => "GenStep";

    protected string name;
    public override string Name => name;

    public AequusGenStep() {
        name = base.Name.Replace("Generator", "").Replace("Step", "");
    }

    public abstract void Apply(GenerationProgress progress, GameConfiguration config);

    public virtual void PostSetupContent() {
    }

    public virtual GenPass CreateGenerationPass() {
        return new PassLegacy(Name, Apply, Weight);
    }

    public virtual void EmergencyOnStepNotFound(List<GenPass> tasks) {
        tasks.Add(CreateGenerationPass());
    }

    public virtual void InsertStep(int index, List<GenPass> tasks) {
        tasks.Insert(index, CreateGenerationPass());
    }

    public virtual void Reset() {
    }

    protected sealed override void Register() {
        Weight = GenWeight;
        WorldGenSystem.GenerationSteps.Add(this);
    }

    public void PostSetupContent(Aequus aequus) {
        this.GetLocalization("DisplayMessage");
        PostSetupContent();
    }

    /// <summary>
    /// Safely sets the progress message.
    /// </summary>
    /// <param name="progress"></param>
    /// <param name="keySuffix"></param>
    protected void SetMessage(GenerationProgress progress, object keySuffix = null) {
        if (progress != null) {
            progress.Message = this.GetLocalizedValue($"DisplayMessage{(keySuffix == null ? "" : "." + keySuffix)}");
        }
    }

    /// <summary>
    /// Safely sets the progress bar.
    /// </summary>
    protected void SetProgress(GenerationProgress progress, double progressValue, double prevProgress = 0f, double wantedProgress = 1f) {
        if (progress != null) {
            progress.Value = (prevProgress + (wantedProgress - prevProgress) * Math.Clamp(progressValue, 0f, 1f)) / Weight;
        }
    }

    protected static void AddStructure(int i, int j, int width, int height, int padding = 0) {
        AddStructure(new Rectangle(i, j, width, height), padding);
    }
    protected static void AddStructure(Rectangle rectangle, int padding = 0) {
        GenVars.structures.AddStructure(rectangle, padding);
    }

    protected static void AddProtectedStructure(int i, int j, int width, int height, int padding = 0) {
        AddProtectedStructure(new Rectangle(i - width / 2, j - height / 2, width, height), padding);
    }
    protected static void AddProtectedStructure(Rectangle rectangle, int padding = 0) {
        GenVars.structures.AddProtectedStructure(rectangle, padding);
    }
}