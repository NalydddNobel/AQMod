using Terraria.ModLoader.IO;

namespace Aequu2.Core.Utilities;

public static class ExtendTagCompound {
    public static T Get<T>(this TagCompound tag, string key, T defaultValue) {
        if (tag.TryGet(key, out T outputValue)) {
            return outputValue;
        }
        return defaultValue;
    }
}