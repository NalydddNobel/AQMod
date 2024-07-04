using AequusRemake.Core.Debug;
using Terraria.Cinematics;

namespace AequusRemake.Content.Cinematics;

internal class CutsceneItem : DebugItem {
    public override bool? UseItem(Player player) {
        CinematicManager.Instance.PlayFilm(new CrabPotFilm());
        return true;
    }
}
