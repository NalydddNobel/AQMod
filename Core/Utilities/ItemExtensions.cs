using Terraria;

namespace Aequus;

public static class ItemExtensions {
    #region Item Lists
    public static int GetNextIndex(this Item[] itemList) {
        int i = 0;
        for (; i < itemList.Length; i++) {
            if (itemList[i] == null) {
                break;
            }
        }
        return i;
    }
    #endregion
}