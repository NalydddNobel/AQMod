using Aequus.Core.DataSets;
using System;
using System.Globalization;
using Terraria.ModLoader.IO;

namespace Aequus.Core.IO;

/// <summary>Helper for loading content which uses a legacy Id system.</summary>
public class IDLoader<T> where T : class {
    public static int LoadId(TagCompound tag, string name, int defaultValue = -1) {
        if (tag.TryGet(name, out object value)) {
            if (value is string idName && IDCommons<T>.Search.TryGetId(idName, out int foundId)) {
                return foundId;
            }

            if (value is IConvertible convertible) {
                return convertible.ToInt32(CultureInfo.InvariantCulture);
            }
        }

        return defaultValue;
    }

    public static void SaveId(TagCompound tag, string name, int id) {
        if (id < IDCommons<T>.Count) {
            tag[name] = id;
        }
        else {
            tag[name] = IDCommons<T>.Search.GetName(id);
        }
    }
}
