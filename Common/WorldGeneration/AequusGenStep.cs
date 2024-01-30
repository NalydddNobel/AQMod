using Aequus.Core.Initialization;
using System;
using System.Collections.Generic;
using Terraria.GameContent.Generation;
using Terraria.IO;
using Terraria.Utilities;
using Terraria.WorldBuilding;

namespace Aequus.Common.WorldGeneration;

public abstract class AequusGenStep : ModType, ILocalizedModType, IPostSetupContent {
    public abstract String InsertAfter { get; }

    public Double Weight { get; private set; }

    public static UnifiedRandom Random => WorldGen.genRand;

    protected virtual Double GenWeight { get; }

    public String LocalizationCategory => "GenStep";

    protected String name;
    public override String Name => name;

    private String _genMessage;

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

    public virtual void InsertStep(Int32 index, List<GenPass> tasks) {
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
    protected void SetMessage(GenerationProgress progress, Object keySuffix = null) {
        if (progress != null) {
            _genMessage = this.GetLocalizedValue($"DisplayMessage{(keySuffix == null ? "" : "." + keySuffix)}");
            progress.Message = _genMessage;
        }
    }

    protected void SetSubMessage(GenerationProgress progress, String key = null) {
        if (progress != null) {
            progress.Message = _genMessage + this.GetLocalizedValue(key);
        }
    }

    /// <summary>Determines the progress of an ij loop in a rectangle. Using 0-<see cref="Main.maxTilesX"/> and 0-<see cref="Main.maxTilesY"/> as the edges.</summary>
    /// <param name="i"></param>
    /// <param name="j"></param>
    protected static Double RectangleProgress(Int32 i, Int32 j) {
        return RectangleProgress(i, j, 0, Main.maxTilesX, 0, Main.maxTilesY);
    }
    /// <summary>Determines the progress of an ij loop in a rectangle. Using 0-<see cref="Main.maxTilesY"/> as the vertical edges.</summary>
    /// <param name="i"></param>
    /// <param name="j"></param>
    /// <param name="leftSide">X coordinate of the Left edge of the rectangle.</param>
    /// <param name="rightSide">X coordinate of the Right edge of the rectnagle.</param>
    protected static Double RectangleProgress(Int32 i, Int32 j, Int32 leftSide, Int32 rightSide) {
        return RectangleProgress(i, j, leftSide, rightSide, 0, Main.maxTilesY);
    }
    /// <summary>Determines the progress of an ij loop in a rectangle.</summary>
    /// <param name="i"></param>
    /// <param name="j"></param>
    /// <param name="leftSide">X coordinate of the Left edge of the rectangle.</param>
    /// <param name="rightSide">X coordinate of the Right edge of the rectnagle.</param>
    /// <param name="topSide">Y coordinate of the Top Edge of the rectangle.</param>
    /// <param name="bottomSide">Y coordinate of the Bottom Edge of the rectangle.</param>
    protected static Double RectangleProgress(Int32 i, Int32 j, Int32 leftSide, Int32 rightSide, Int32 topSide, Int32 bottomSide) {
        return (i * Main.maxTilesY + j) / (Double)(rightSide - leftSide + (bottomSide - topSide));
    }

    /// <summary>
    /// Safely sets the progress bar.
    /// </summary>
    protected void SetProgress(GenerationProgress progress, Double progressValue, Double prevProgress = 0f, Double wantedProgress = 1f) {
        if (progress != null) {
            progress.Value = (prevProgress + (wantedProgress - prevProgress) * Math.Clamp(progressValue, 0f, 1f)) / Weight;
        }
    }

    protected static void AddStructure(Int32 i, Int32 j, Int32 width, Int32 height, Int32 padding = 0) {
        AddStructure(new Rectangle(i, j, width, height), padding);
    }
    protected static void AddStructure(Rectangle rectangle, Int32 padding = 0) {
        GenVars.structures.AddStructure(rectangle, padding);
    }

    protected static void AddProtectedStructure(Int32 i, Int32 j, Int32 width, Int32 height, Int32 padding = 0) {
        AddProtectedStructure(new Rectangle(i - width / 2, j - height / 2, width, height), padding);
    }
    protected static void AddProtectedStructure(Rectangle rectangle, Int32 padding = 0) {
        GenVars.structures.AddProtectedStructure(rectangle, padding);
    }
}