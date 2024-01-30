using Terraria.ModLoader.IO;

namespace Aequus.Core.Utilities;

public static class ExtendTagCompound {
    public static T Get<T>(this TagCompound tag, System.String key, T defaultValue) {
        if (tag.TryGet(key, out T outputValue)) {
            return outputValue;
        }
        return defaultValue;
    }
}