using Terraria.DataStructures;
using Terraria.Localization;

namespace Aequu2.Core.Hooks;

public partial class TerrariaHooks {
    /// <summary>Translates Aequu2' death text. Since this isn't supported yet for some dumb reason.</summary>
    private static NetworkText On_PlayerDeathReason_GetDeathText(On_PlayerDeathReason.orig_GetDeathText orig, PlayerDeathReason deathReason, string deadPlayerName) {
        if (deathReason.SourceCustomReason != null && deathReason.SourceCustomReason.StartsWith("Mods.Aequu2.")) {
            // Return this custom network text for reasons which start with "Mods.Aequu2"
            return NetworkText.FromKey(deathReason.SourceCustomReason, deadPlayerName);
        }

        return orig(deathReason, deadPlayerName);
    }
}
