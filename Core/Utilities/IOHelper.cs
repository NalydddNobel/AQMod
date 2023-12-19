using Terraria.ModLoader.IO;

namespace Aequus.Core.Utilities;

public static class IOHelper {
    public static T Get<T>(this TagCompound tag, string key, T defaultValue) {
        if (tag.TryGet(key, out T outputValue)) {
            return outputValue;
        }
        return defaultValue;
    }
}