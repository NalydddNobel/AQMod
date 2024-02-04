using ReLogic.Reflection;
using System;
using System.Globalization;
using Terraria.ModLoader.IO;

namespace Aequus.Core.IO;

/// <summary>
/// Helper for loading content which uses a legacy Id system.
/// </summary>
public class IDLoader<T> where T : class {
    private static readonly IdDictionary _idDictionary = (IdDictionary)typeof(T).GetField("Search").GetValue(null);
    private static readonly int _vanillaCount = Convert.ToInt32(typeof(T).GetField("Count").GetValue(null));

    public static int LoadId(TagCompound tag, string name, int defaultValue = -1) {
        if (tag.TryGet(name, out object value)) {
            if (value is string idName && _idDictionary.TryGetId(idName, out int foundId)) {
                return foundId;
            }

            if (value is IConvertible convertible) {
                return convertible.ToInt32(CultureInfo.InvariantCulture);
            }
        }

        return defaultValue;
    }

    public static void SaveId(TagCompound tag, string name, int id) {
        if (id < _vanillaCount) {
            tag[name] = id;
        }
        else {
            tag[name] = _idDictionary.GetName(id);
        }
    }
}
