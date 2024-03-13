using ReLogic.Reflection;
using System;
using System.Reflection;

namespace Aequus.Core.DataSets;

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
