using ReLogic.Reflection;
using System;
using System.Globalization;
using System.Reflection;
using Terraria.ModLoader.IO;

namespace Aequus.Common.IO;

public class IDCommons {
    public static bool LoadRarityFromTag(TagCompound tag, string key, out int result) {
        if (tag.TryGet(key, out object loadedValue)) {
            if (loadedValue is string idName && ItemRarityID.Search.TryGetId(idName, out int foundId)) {
                result = foundId;
                return true;
            }

            if (loadedValue is IConvertible convertible) {
                result = convertible.ToInt32(CultureInfo.InvariantCulture);
                return true;
            }
        }

        result = ItemRarityID.White;
        return false;
    }

    public static void SaveRarityToTag(TagCompound tag, string key, int id) {
        if (id < ItemRarityID.Count) {
            tag[key] = id;
        }
        else {
            tag[key] = ItemRarityID.Search.GetName(id);
        }
    }
}

public static class IDCommons<T> where T : class {
    public static readonly IdDictionary Search = (IdDictionary)typeof(T).GetField("Search").GetValue(null);
    /// <summary>The vanilla count.</summary>
    public static readonly int Count = Convert.ToInt32(typeof(T).GetField("Count", BindingFlags.Public | BindingFlags.Static).GetValue(null));
    /// <summary>The starting Id. This is usually 0, but in some cases like <see cref="NPCID"/> it will be equal to <see cref="NPCID.NegativeIDCount"/>.</summary>
    public static readonly int StartCount = GetStartCount();

    private static int GetStartCount() {
        Type t = typeof(T);
        FieldInfo negativeIdField = t.GetField("NegativeIDCount", BindingFlags.Public | BindingFlags.Static);

        if (negativeIdField == null) {
            return 0;
        }

        object value = negativeIdField.GetValue(null);
        return value == null ? 0 : Convert.ToInt32(value);
    }

    public static string GetStringIdentifier(int id) {
        return id < Count ? id.ToString() : Search.GetName(id);
    }
}
