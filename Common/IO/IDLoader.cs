using System;
using System.Globalization;
using Terraria.ModLoader.IO;

namespace Aequus.Common.IO;

/// <summary>Helper for loading content which uses a legacy Id system.</summary>
public class IDLoader<T> where T : class {
    #region Tag Compound
    public static int LoadFromTag(TagCompound tag, string key, int defaultValue = -1) {
        if (tag.TryGet(key, out object value)) {
            if (value is string idName) {
                if (IDCommons<T>.Search.TryGetId(idName, out int foundId)) {
                    return foundId;
                }

                return defaultValue;
            }

            if (value is IConvertible convertible) {
                return convertible.ToInt32(CultureInfo.InvariantCulture);
            }
        }

        return defaultValue;
    }

    public static void SaveToTag(TagCompound tag, string key, int id) {
        if (id < IDCommons<T>.Count) {
            tag[key] = id;
        }
        else {
            tag[key] = IDCommons<T>.Search.GetName(id);
        }
    }
    #endregion
}
