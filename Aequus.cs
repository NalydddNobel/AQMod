using log4net;
using System.Reflection;
using Terraria.Graphics.Effects;
using Terraria.Localization;
using Terraria.Utilities;

namespace Aequus;

public partial class Aequus : Mod {
    public static Aequus Instance { get; private set; }
    public static Mod MusicMod { get; private set; }

    /// <summary>Shorthand for <see cref="Instance"/>.Logger.</summary>
    public static ILog Log => Instance.Logger;

    /// <summary>Shorthand for typeof(<see cref="Main"/>).Assembly.</summary>
    public static Assembly TerrariaAssembly => typeof(Main).Assembly;

    /// <summary>Shorthand for checking if <see cref="Main.gfxQuality"/> is greater than 0.</summary>
    public static bool HighQualityEffects => Main.gfxQuality > 0f;

    /// <summary>Shorthand for checking if <see cref="Lighting.NotRetro"/> is true, and <see cref="FilterManager.CanCapture"/>.</summary>
    public static bool ScreenShadersActive => Lighting.NotRetro && Filters.Scene.CanCapture();

    /// <summary>Shorthand for a bunch of checks determining whether the game is unpaused.</summary>
    public static bool GameWorldActive => (Main.instance.IsActive && !Main.gamePaused && !Main.gameInactive) || Main.netMode != NetmodeID.SinglePlayer;

    public override void Load() {
        Instance = this;
        MusicMod = ModLoader.GetMod("AequusMusic");
        LoadModCalls();
    }

    public override void Unload() {
        Instance = null;
        MusicMod = null;
        UnloadLoadingSteps();
        UnloadModCalls();
        UnloadPackets();
    }

    /// <returns>A random name. Used in naturally generated underworld tombstones.</returns>
    public static LocalizedText GetRandomName(UnifiedRandom random = null) {
        string filter = "Mods.Aequus.Names";
        return Language.SelectRandom((key, value) => key.StartsWith(filter), random);
    }
}