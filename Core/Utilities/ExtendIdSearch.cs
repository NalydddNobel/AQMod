using ReLogic.Reflection;

namespace AequusRemake.Core.Utilities;

public static class ExtendIdSearch {
    public static string GetNPCName(int Id) {
        return Get<NPCID>(Id, NPCLoader.GetNPC(Id), NPCID.Search);
    }

    private static string Get<TID>(int Id, IModType modType, IdDictionary idDictionary) where TID : class {
        if (idDictionary.TryGetName(Id, out string name)) {
            return name.Replace("AequusRemake/", "");
        }

        if (modType != null) {
            return modType.Name;
        }

        return $"Unknown{Id}";
    }
}
