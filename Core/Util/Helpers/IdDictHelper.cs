using ReLogic.Reflection;

namespace AequusRemake.Core.Util.Helpers;

public sealed class IdDictHelper {
    /// <param name="Type"></param>
    /// <returns>The internal name of an Item of <paramref name="Type"/>.</returns>
    public static string GetItemName(int Type) {
        return GetNameInner(Type, ItemLoader.GetItem(Type), ItemID.Search);
    }

    /// <param name="Type"></param>
    /// <returns>The internal name of an NPC of <paramref name="Type"/>.</returns>
    public static string GetNPCName(int Type) {
        return GetNameInner(Type, NPCLoader.GetNPC(Type), NPCID.Search);
    }

    private static string GetNameInner(int Id, IModType modType, IdDictionary idDictionary) {
        if (idDictionary.TryGetName(Id, out string name)) {
            return name.Replace("AequusRemake/", "");
        }

        if (modType != null) {
            return modType.Name;
        }

        return $"Unknown{Id}";
    }
}
