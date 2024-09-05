﻿using System;
using System.Collections.Generic;
using System.Threading;
using Terraria.GameContent.Generation;
using Terraria.IO;
using Terraria.Utilities;
using Terraria.WorldBuilding;

namespace Aequus.Common.World;
public abstract class Generator : ModType, ILocalizedModType {
    private GenerationProgress progress;
    private GameConfiguration config;
    internal bool generating;

    public bool Generating => generating;
    protected UnifiedRandom Rand => WorldGen.genRand;
    public virtual float Weight => 1f;

    string ILocalizedModType.LocalizationCategory => "GenStep";

    protected sealed override void Register() {
        AequusWorldGenerator.Generators ??= new List<Generator>();
        AequusWorldGenerator.Generators.Add(this);
    }

    public sealed override void SetupContent() {
        SetStaticDefaults();
    }

    public void GenerateOnThread(GenerationProgress progress = null, GameConfiguration config = null) {
        if (WorldGen.gen) {
            Generate(progress, config);
            return;
        }

        this.progress = progress;
        this.config = config;
        generating = true;
        ThreadPool.QueueUserWorkItem((obj) => {
            var me = (Generator)obj;
            try {
                me.Generate();
            }
            catch (Exception ex) {
                Main.QueueMainThreadAction(() => {
                    me.Mod.Logger.Error($"{me.Name} failed when conducting generation...");
                    me.Mod.Logger.Error($"{ex.Message}\n{ex.StackTrace}");
                });
            }
            me.generating = false;
            me.progress = null;
            me.config = null;
        }, this);
    }

    public void SetGenerationValues(GenerationProgress progress, GameConfiguration config) {
        this.progress = progress;
        this.config = config;
    }

    public void Generate(GenerationProgress progress = null, GameConfiguration config = null) {
        SetGenerationValues(progress, config);
        generating = true;
        try {
            Generate();
        }
        catch (Exception ex) {
            Mod.Logger.Error($"{Name} failed when conducting generation...");
            Mod.Logger.Error(ex);
            if (WorldGen.gen) {
                throw ex;
            }
        }
        generating = false;
        SetGenerationValues(null, null);
    }

    protected abstract void Generate();

    public virtual void Initialize() {
    }

    public virtual void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight) {
    }

    protected void AddPass(string task, string myName, WorldGenLegacyMethod generation, List<GenPass> tasks) {
        int i = tasks.FindIndex((t) => t.Name.Equals(task));
        var pass = new PassLegacy("Aequus: " + myName, generation);
        if (i != -1) {
            tasks.Insert(i + 1, pass);
        }
    }


    protected void SetTextValue(string text) {
        if (progress != null) {
            progress.Message = text;
        }
    }

    protected void SetText(string key) {
        SetTextValue(TextHelper.GetTextValue(key));
    }

    protected void SetProgress(float progress) {
        if (this.progress != null) {
            this.progress.Value = progress;
        }
    }
}
