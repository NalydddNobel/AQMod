using ReLogic.Reflection;
using Terraria.ModLoader.IO;

namespace Aequus.Core;

/// <summary>
/// Helper for loading content which uses a legacy Id system.
/// </summary>
public class IDLoader<T> where T : class {
    private static readonly IdDictionary _idDictionary = (IdDictionary)typeof(T).GetField("Search").GetValue(null);
    private static readonly System.Int32 _vanillaCount = (System.Int32)typeof(T).GetField("Count").GetValue(null);

    public static System.Int32 LoadId(TagCompound tag, System.String name, System.Int32 defaultValue = -1) {
        if (tag.TryGet(name, out System.Int32 id)) {
            return id;
        }

        if (!tag.TryGet(name, out System.String idName) || !_idDictionary.TryGetId(idName, out System.Int32 foundId)) {
            return defaultValue;
        }

        return foundId;
    }

    public static void SaveId(TagCompound tag, System.String name, System.Int32 id) {
        if (id < _vanillaCount) {
            tag[name] = id;
        }
        else {
            tag[name] = _idDictionary.GetName(id);
        }
    }
}
