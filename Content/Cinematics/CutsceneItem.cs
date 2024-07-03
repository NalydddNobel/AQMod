using Aequu2.Core.Debug;
using Terraria.Cinematics;

namespace Aequu2.Content.Cinematics;

internal class CutsceneItem : DebugItem {
    public override bool? UseItem(Player player) {
        CinematicManager.Instance.PlayFilm(new CrabPotFilm());
        return true;
    }
}
