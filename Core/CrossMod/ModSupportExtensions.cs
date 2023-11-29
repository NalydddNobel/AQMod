using System.Collections.Generic;
using Terraria.ModLoader;

namespace Aequus.Core.CrossMod;

internal static class ModSupportExtensions {
    public static bool LoadingAllowed<TMod>(this IModSupport<TMod> modSupport) where TMod : ModSupport<TMod> {
        return modSupport.IsLoadingEnabled();
    }

    public static bool TryFind<T, TMod>(this IModSupport<TMod> modSupport, string name, out T value) where T : IModType where TMod : ModSupport<TMod> {
        var instance = ModSupport<TMod>.Instance;
        if (instance == null) {
            value = default(T);
            return false;
        }
        return instance.TryFind(name, out value);
    }

    public static bool AddItemToSet<TMod>(this IModSupport<TMod> modSupport, string name, HashSet<int> hashSet) where TMod : ModSupport<TMod> {
        if (!modSupport.TryFind<ModItem, TMod>(name, out var modItem)) {
            return false;
        }

        hashSet.Add(modItem.Type);
        return true;
    }

    public static int GetItem<TMod>(this IModSupport<TMod> modSupport, string name, int defaultItem = 0) where TMod : ModSupport<TMod> {
        return modSupport.TryFind<ModItem, TMod>(name, out var value) ? value.Type : defaultItem;
    }
    public static int GetNPC<TMod>(this IModSupport<TMod> modSupport, string name, int defaultNPC = 0) where TMod : ModSupport<TMod> {
        return modSupport.TryFind<ModNPC, TMod>(name, out var value) ? value.Type : defaultNPC;
    }
}