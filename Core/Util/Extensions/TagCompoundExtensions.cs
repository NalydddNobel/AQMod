using Terraria.ModLoader.IO;

namespace AequusRemake.Core.Util.Extensions;

public static class TagCompoundExtensions {
    public static T Get<T>(this TagCompound tag, string key, T defaultValue) {
        if (tag.TryGet(key, out T outputValue)) {
            return outputValue;
        }
        return defaultValue;
    }
}