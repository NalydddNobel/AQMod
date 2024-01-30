using Aequus.Core.CrossMod;
using Terraria.Localization;

namespace Aequus.Content.CrossMod;

// https://steamcommunity.com/sharedfiles/filedetails/?id=2985966860
// https://github.com/GabeHasWon/MusicDisplay/blob/master/README.md For support documentation.
internal class MusicDisplay : SupportedMod<MusicDisplay> {
    public override void PostSetupContent() {
        RegisterMusic("PollutedOcean", "extra");

        static void RegisterMusic(System.String musicName, System.String musicAuthor) {
            Instance.Call(
                "AddMusic",
                (System.Int16)MusicLoader.GetMusicSlot($"Aequus/Assets/Music/{musicName}"), // Music Id, must be casted to a short
                Language.GetText($"Mods.Aequus.Music.{musicName}"), // Name of the song.
                Language.GetText($"Mods.Aequus.Music.Authors.{musicAuthor}"), // Name of the author
                "Aequus" // Name of the mod, this will not have any translations.
            );
        }
    }
}
