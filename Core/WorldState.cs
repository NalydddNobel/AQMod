using Aequus.Core.IO;
using System.Reflection;
using Terraria.ModLoader.IO;

namespace Aequus.Core;

public class WorldState : ModSystem {
    #region Tile Ranges
    /// <summary>Returns a point with the beginning (left) (X) and end (right) (Y) of the 'safe' underworld in remix worlds. This does not represent a point in 2D space, but rather two X coordinates.</summary>
    public static Point RemixWorldSafeUnderworldRange => new Point((int)(Main.maxTilesX * 0.38) + 50, (int)(Main.maxTilesX * 0.62));
    #endregion

    [SaveData("Aquatic")]
    internal static bool _downedAquaticBoss;
    [SaveData("Cosmic")]
    internal static bool _downedCosmicBoss;
    [SaveData("Cosmic2")]
    internal static bool _downedTrueCosmicBoss;
    [SaveData("Demon")]
    internal static bool _downedDemonBoss;
    [SaveData("Demon2")]
    internal static bool _downedTrueDemonBoss;
    [SaveData("Flame")]
    internal static bool _downedAtmoBossFlame;
    [SaveData("Frost")]
    internal static bool _downedAtmoBossFrost;
    [SaveData("Atmo")]
    internal static bool _downedTrueAtmoBoss;
    [SaveData("Might")]
    internal static bool _downedOrganicBossMight;
    [SaveData("Sight")]
    internal static bool _downedOrganicBossSight;
    [SaveData("Fright")]
    internal static bool _downedOrganicBossFright;
    [SaveData("Final")]
    internal static bool _downedTrueFinalBoss;

    /// <summary>Whether the Occultist has been met. This determines if the Hostile Occultist emote can be used.</summary>
    [SaveData("MetOccultist")]
    internal static bool _metOccultist;

    public static bool DownedAquaticBoss { get => _downedAquaticBoss; }
    public static bool DownedCosmicBoss { get => _downedCosmicBoss; }
    public static bool DownedTrueCosmicBoss { get => _downedTrueCosmicBoss; }
    public static bool DownedDemonBoss { get => _downedDemonBoss; }
    public static bool DownedTrueDemonBoss { get => _downedTrueDemonBoss; }
    public static bool DownedAtmoBossFlame { get => _downedAtmoBossFlame; }
    public static bool DownedAtmoBossFrost { get => _downedAtmoBossFrost; }
    public static bool DownedTrueAtmoBoss { get => _downedTrueAtmoBoss; }
    public static bool DownedOrganicBossMight { get => _downedOrganicBossMight; }
    public static bool DownedOrganicBossSight { get => _downedOrganicBossSight; }
    public static bool DownedOrganicBossFright { get => _downedOrganicBossFright; }
    public static bool DownedTrueFinalBoss { get => _downedTrueFinalBoss; }

    [SaveData("ReforgeBook")]
    public static bool UsedReforgeBook;

    [SaveData("BuriedChestSeed")]
    public static int BuriedChestsLooted;

    public static bool HardmodeTier => Main.hardMode || DownedTrueCosmicBoss;

    public override void SaveWorldData(TagCompound tag) {
        SaveDataAttribute.SaveData(tag, this, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
    }

    public override void LoadWorldData(TagCompound tag) {
        SaveDataAttribute.LoadData(tag, this, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
    }
}
