using Aequus.Content.Cinematics;
using Aequus.Core.Debugging;
using Terraria.Cinematics;

namespace Aequus.Content.Cutscenes;

internal class CutsceneItem : DebugItem {
    public override bool? UseItem(Player player) {
        CinematicManager.Instance.PlayFilm(new CrabPotFilm());
        return true;
    }
}
