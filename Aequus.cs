using System;
using Terraria.ModLoader;

namespace Aequus; 

public class Aequus : Mod {
    public static Aequus Instance { get; private set; }
    public static bool highQualityEffects = true;

    public override void Load() {
        Instance = this;
    }

    public override void Unload() {
        Instance = null;
    }
}