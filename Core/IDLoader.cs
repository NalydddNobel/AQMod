using ReLogic.Reflection;
using Terraria.ModLoader.IO;

namespace Aequus.Core;

/// <summary>
/// Helper for loading content which uses a legacy Id system.
/// </summary>
public class IDLoader<T> where T : class {
    private static readonly IdDictionary _idDictionary = (IdDictionary)typeof(T).GetField("Search").GetValue(null);
    private static readonly int _vanillaCount = (int)typeof(T).GetField("Count").GetValue(null);

    public static int LoadId(TagCompound tag, string name, int defaultValue = -1) {
        if (tag.TryGet(name, out int id)) {
            return id;
        }

        if (!tag.TryGet(name, out string idName) || !_idDictionary.TryGetId(idName, out int foundId)) {
            return defaultValue;
        }

        return foundId;
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
