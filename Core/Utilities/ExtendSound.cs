using Terraria.Audio;

namespace Aequus.Core.Utilities;

public static class ExtendSound {
    /// <returns>The Sound's path with the first directory removed.</returns>
    public static System.String ModPath(this SoundStyle sound) => System.String.Join('/', sound.SoundPath.Split('/')[1..]);
}
