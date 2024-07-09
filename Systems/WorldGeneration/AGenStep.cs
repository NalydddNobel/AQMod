using System;
using System.Collections.Generic;
using Terraria.GameContent.Generation;
using Terraria.IO;
using Terraria.Utilities;
using Terraria.WorldBuilding;
using tModLoaderExtended.Terraria.ModLoader;

namespace AequusRemake.Systems.WorldGeneration;

public abstract class AGenStep : ModType, ILocalizedModType, IPostSetupContent {
    public abstract string InsertAfter { get; }

    public double Weight { get; private set; } = 1f;

    public static UnifiedRandom Random => WorldGen.genRand;

    protected virtual double GenWeight => 1f;

    public string LocalizationCategory => "GenStep";

    protected string name;
    public override string Name => name;

    private string _genMessage;

    public AGenStep() {
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

    public void PostSetupContent(Mod mod) {
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
            _genMessage = this.GetLocalizedValue($"DisplayMessage{(keySuffix == null ? "" : "." + keySuffix)}");
            progress.Message = _genMessage;
        }
    }

    protected void SetSubMessage(GenerationProgress progress, string key = null) {
        if (progress != null) {
            progress.Message = _genMessage + this.GetLocalizedValue(key);
        }
    }

    /// <summary>Determines the progress of an ij loop in a rectangle. Using 0-<see cref="Main.maxTilesX"/> and 0-<see cref="Main.maxTilesY"/> as the edges.</summary>
    /// <param name="i"></param>
    /// <param name="j"></param>
    protected static double RectangleProgress(int i, int j) {
        return RectangleProgress(i, j, 0, Main.maxTilesX, 0, Main.maxTilesY);
    }
    /// <summary>Determines the progress of an ij loop in a rectangle. Using 0-<see cref="Main.maxTilesY"/> as the vertical edges.</summary>
    /// <param name="i"></param>
    /// <param name="j"></param>
    /// <param name="leftSide">X coordinate of the Left edge of the rectangle.</param>
    /// <param name="rightSide">X coordinate of the Right edge of the rectnagle.</param>
    protected static double RectangleProgress(int i, int j, int leftSide, int rightSide) {
        return RectangleProgress(i, j, leftSide, rightSide, 0, Main.maxTilesY);
    }
    /// <summary>Determines the progress of an ij loop in a rectangle.</summary>
    /// <param name="i"></param>
    /// <param name="j"></param>
    /// <param name="leftSide">X coordinate of the Left edge of the rectangle.</param>
    /// <param name="rightSide">X coordinate of the Right edge of the rectnagle.</param>
    /// <param name="topSide">Y coordinate of the Top Edge of the rectangle.</param>
    /// <param name="bottomSide">Y coordinate of the Bottom Edge of the rectangle.</param>
    protected static double RectangleProgress(int i, int j, int leftSide, int rightSide, int topSide, int bottomSide) {
        int height = bottomSide - topSide;
        int width = rightSide - leftSide;
        int startX = i - leftSide;
        int startY = j - topSide;
        int progress = startX * height + startY;
        double maximumProgress = width * height;
        return progress / maximumProgress;
    }

    /// <summary>
    /// Safely sets the progress bar.
    /// </summary>
    protected void SetProgress(GenerationProgress progress, double progressValue, double prevProgress = 0f, double wantedProgress = 1f) {
        if (progress != null) {
            progress.Value = prevProgress + (wantedProgress - prevProgress) * Math.Clamp(progressValue, 0f, 1f);
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