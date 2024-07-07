using Terraria.Audio;

namespace AequusRemake.Core.Util.Extensions;

public static class SoundStyleExtensions {
    /// <returns>The Sound's path with the first directory removed.</returns>
    public static string ModPath(this SoundStyle sound) {
        return string.Join('/', sound.SoundPath.Split('/')[1..]);
    }
}
