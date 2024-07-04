using Terraria.Audio;

namespace AequusRemake.Core.Utilities;

public static class ExtendSound {
    /// <returns>The Sound's path with the first directory removed.</returns>
    public static string ModPath(this SoundStyle sound) {
        return string.Join('/', sound.SoundPath.Split('/')[1..]);
    }
}
